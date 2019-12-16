using DFC.App.JobProfileSkills.ApiModels;
using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using DFC.App.JobProfileSkills.Extensions;
using DFC.App.JobProfileSkills.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.Controllers
{
    public class SegmentController : Controller
    {
        private const string IndexActionName = nameof(Index);
        private const string DocumentActionName = nameof(Document);
        private const string BodyActionName = nameof(Body);
        private const string PutActionName = nameof(Put);
        private const string PostActionName = nameof(Post);
        private const string DeleteActionName = nameof(Delete);
        private const string PatchDigitalSkillActionName = nameof(PatchDigitalSkill);
        private const string PatchOnetSkillActionName = nameof(PatchOnetSkill);
        private const string PatchSkillsMatrixActionName = nameof(PatchSkillsMatrix);
        private const string PatchRestrictionActionName = nameof(PatchRestriction);

        private readonly ILogService logService;
        private readonly ISkillSegmentService skillSegmentService;
        private readonly AutoMapper.IMapper mapper;

        public SegmentController(ILogService logService, ISkillSegmentService skillSegmentService, AutoMapper.IMapper mapper)
        {
            this.logService = logService;
            this.skillSegmentService = skillSegmentService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/")]
        [Route("{controller}")]
        public async Task<IActionResult> Index()
        {
            logService.LogInformation($"{IndexActionName} has been called");

            var viewModel = new IndexViewModel();
            var segmentModels = await skillSegmentService.GetAllAsync().ConfigureAwait(false);

            if (segmentModels != null)
            {
                viewModel.Documents = segmentModels
                    .OrderBy(x => x.CanonicalName)
                    .Select(x => mapper.Map<IndexDocumentViewModel>(x))
                    .ToList();

                logService.LogInformation($"{IndexActionName} has succeeded");
            }
            else
            {
                logService.LogWarning($"{IndexActionName} has returned with no results");
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("{controller}/{article}")]
        public async Task<IActionResult> Document(string article)
        {
            logService.LogInformation($"{DocumentActionName} has been called with: {article}");

            var model = await skillSegmentService.GetByNameAsync(article).ConfigureAwait(false);

            if (model != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(model);

                logService.LogInformation($"{DocumentActionName} has succeeded for: {article}");

                return View(nameof(Body), viewModel);
            }

            logService.LogWarning($"{DocumentActionName} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("{controller}/{documentId}/contents")]
        public async Task<IActionResult> Body(Guid documentId)
        {
            logService.LogInformation($"{BodyActionName} has been called with: {documentId}");

            var model = await skillSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (model != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(model);

                logService.LogInformation($"{BodyActionName} has succeeded for: {documentId}");

                var apiModel = mapper.Map<WhatItTakesApiModel>(model.Data);

                return this.NegotiateContentResult(viewModel, apiModel);
            }

            logService.LogWarning($"{BodyActionName} has returned no content for: {documentId}");

            return NoContent();
        }

        [HttpPost]
        [Route("segment")]
        public async Task<IActionResult> Post([FromBody]JobProfileSkillSegmentModel jobProfileSkillSegmentModel)
        {
            logService.LogInformation($"{PostActionName} has been called");

            if (jobProfileSkillSegmentModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDocument = await skillSegmentService.GetByIdAsync(jobProfileSkillSegmentModel.DocumentId).ConfigureAwait(false);
            if (existingDocument != null)
            {
                return new StatusCodeResult((int)HttpStatusCode.AlreadyReported);
            }

            var response = await skillSegmentService.UpsertAsync(jobProfileSkillSegmentModel).ConfigureAwait(false);

            logService.LogInformation($"{PostActionName} has upserted content for: {jobProfileSkillSegmentModel.CanonicalName}");

            return new StatusCodeResult((int)response);
        }

        [HttpPut]
        [Route("segment")]
        public async Task<IActionResult> Put([FromBody]JobProfileSkillSegmentModel jobProfileSkillSegmentModel)
        {
            logService.LogInformation($"{PutActionName} has been called");

            if (jobProfileSkillSegmentModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDocument = await skillSegmentService.GetByIdAsync(jobProfileSkillSegmentModel.DocumentId).ConfigureAwait(false);
            if (existingDocument == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            if (jobProfileSkillSegmentModel.SequenceNumber <= existingDocument.SequenceNumber)
            {
                return new StatusCodeResult((int)HttpStatusCode.AlreadyReported);
            }

            jobProfileSkillSegmentModel.Etag = existingDocument.Etag;
            jobProfileSkillSegmentModel.SocLevelTwo = existingDocument.SocLevelTwo;

            var response = await skillSegmentService.UpsertAsync(jobProfileSkillSegmentModel).ConfigureAwait(false);

            return new StatusCodeResult((int)response);
        }

        [HttpPatch]
        [Route("segment/{documentId}/digitalSkillsLevel")]
        public async Task<IActionResult> PatchDigitalSkill([FromBody]PatchDigitalSkillModel patchDigitalSkillModel, Guid documentId)
        {
            logService.LogInformation($"{PatchDigitalSkillActionName} has been called");

            if (patchDigitalSkillModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await skillSegmentService.PatchDigitalSkillAsync(patchDigitalSkillModel, documentId).ConfigureAwait(false);
            if (response != HttpStatusCode.OK && response != HttpStatusCode.Created)
            {
                logService.LogError($"{PatchDigitalSkillActionName}: Error while patching Digital Skill Level content for Job Profile with Id: {patchDigitalSkillModel.JobProfileId} for {patchDigitalSkillModel.Title} ");
            }

            return new StatusCodeResult((int)response);
        }

        [HttpPatch]
        [Route("segment/{documentId}/onetSkill")]
        public async Task<IActionResult> PatchOnetSkill([FromBody]PatchOnetSkillModel patchOnetSkillModel, Guid documentId)
        {
            logService.LogInformation($"{PatchOnetSkillActionName} has been called");

            if (patchOnetSkillModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await skillSegmentService.PatchOnetSkillAsync(patchOnetSkillModel, documentId).ConfigureAwait(false);
            if (response != HttpStatusCode.OK && response != HttpStatusCode.Created)
            {
                logService.LogError($"{PatchOnetSkillActionName}: Error while patching Related Skill content for Job Profile with Id: {patchOnetSkillModel.JobProfileId} for {patchOnetSkillModel.Title} ");
            }

            return new StatusCodeResult((int)response);
        }

        [HttpPatch]
        [Route("segment/{documentId}/skillsMatrix")]
        public async Task<IActionResult> PatchSkillsMatrix([FromBody]PatchContextualisedModel patchContextualisedModel, Guid documentId)
        {
            logService.LogInformation($"{PatchSkillsMatrixActionName} has been called");

            if (patchContextualisedModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await skillSegmentService.PatchSkillsMatrixAsync(patchContextualisedModel, documentId).ConfigureAwait(false);
            if (response != HttpStatusCode.OK && response != HttpStatusCode.Created)
            {
                logService.LogError($"{PatchSkillsMatrixActionName}: Error while patching Skills Matrix content for Job Profile with Id: {patchContextualisedModel.JobProfileId} for {patchContextualisedModel.Description} ");
            }

            return new StatusCodeResult((int)response);
        }

        [HttpPatch]
        [Route("segment/{documentId}/restriction")]
        public async Task<IActionResult> PatchRestriction([FromBody]PatchRestrictionModel patchRestrictionModel, Guid documentId)
        {
            logService.LogInformation($"{PatchRestrictionActionName} has been called");

            if (patchRestrictionModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await skillSegmentService.PatchRestrictionAsync(patchRestrictionModel, documentId).ConfigureAwait(false);
            if (response != HttpStatusCode.OK && response != HttpStatusCode.Created)
            {
                logService.LogError($"{PatchRestrictionActionName}: Error while patching Skills Matrix content for Job Profile with Id: {patchRestrictionModel.JobProfileId} for {patchRestrictionModel.Title} ");
            }

            return new StatusCodeResult((int)response);
        }

        [HttpDelete]
        [Route("{controller}/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            logService.LogInformation($"{DeleteActionName} has been called");

            var isDeleted = await skillSegmentService.DeleteAsync(documentId).ConfigureAwait(false);
            if (isDeleted)
            {
                logService.LogInformation($"{DeleteActionName} has deleted content for document Id: {documentId}");
                return Ok();
            }
            else
            {
                logService.LogWarning($"{DeleteActionName} has returned no content for: {documentId}");
                return NotFound();
            }
        }
    }
}