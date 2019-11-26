using DFC.App.JobProfileSkills.ApiModels;
using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Data.Models.PatchModels;
using DFC.App.JobProfileSkills.Extensions;
using DFC.App.JobProfileSkills.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<SegmentController> logger;
        private readonly ISkillSegmentService skillSegmentService;
        private readonly AutoMapper.IMapper mapper;

        public SegmentController(ILogger<SegmentController> logger, ISkillSegmentService skillSegmentService, AutoMapper.IMapper mapper)
        {
            this.logger = logger;
            this.skillSegmentService = skillSegmentService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/")]
        [Route("{controller}")]
        public async Task<IActionResult> Index()
        {
            logger.LogInformation($"{IndexActionName} has been called");

            var viewModel = new IndexViewModel();
            var segmentModels = await skillSegmentService.GetAllAsync().ConfigureAwait(false);

            if (segmentModels != null)
            {
                viewModel.Documents = segmentModels
                    .OrderBy(x => x.CanonicalName)
                    .Select(x => mapper.Map<IndexDocumentViewModel>(x))
                    .ToList();

                logger.LogInformation($"{IndexActionName} has succeeded");
            }
            else
            {
                logger.LogWarning($"{IndexActionName} has returned with no results");
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("{controller}/{article}")]
        public async Task<IActionResult> Document(string article)
        {
            logger.LogInformation($"{DocumentActionName} has been called with: {article}");

            var model = await skillSegmentService.GetByNameAsync(article).ConfigureAwait(false);

            if (model != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(model);

                logger.LogInformation($"{DocumentActionName} has succeeded for: {article}");

                return View(viewModel);
            }

            logger.LogWarning($"{DocumentActionName} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("{controller}/{documentId}/contents")]
        public async Task<IActionResult> Body(Guid documentId)
        {
            logger.LogInformation($"{BodyActionName} has been called with: {documentId}");

            var model = await skillSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (model != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(model);

                logger.LogInformation($"{BodyActionName} has succeeded for: {documentId}");

                var apiModel = mapper.Map<WhatItTakesApiModel>(model.Data);

                return this.NegotiateContentResult(viewModel, apiModel);
            }

            logger.LogWarning($"{BodyActionName} has returned no content for: {documentId}");

            return NoContent();
        }

        [HttpPost]
        [Route("segment")]
        public async Task<IActionResult> Post([FromBody]JobProfileSkillSegmentModel jobProfileSkillSegmentModel)
        {
            logger.LogInformation($"{PostActionName} has been called");

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

            logger.LogInformation($"{PostActionName} has upserted content for: {jobProfileSkillSegmentModel.CanonicalName}");

            return new StatusCodeResult((int)response);
        }

        [HttpPut]
        [Route("segment")]
        public async Task<IActionResult> Put([FromBody]JobProfileSkillSegmentModel jobProfileSkillSegmentModel)
        {
            logger.LogInformation($"{PutActionName} has been called");

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
            logger.LogInformation($"{PatchDigitalSkillActionName} has been called");

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
                logger.LogError($"{PatchDigitalSkillActionName}: Error while patching Digital Skill Level content for Job Profile with Id: {patchDigitalSkillModel.JobProfileId} for {patchDigitalSkillModel.Title} ");
            }

            return new StatusCodeResult((int)response);
        }

        [HttpPatch]
        [Route("segment/{documentId}/onetSkill")]
        public async Task<IActionResult> PatchOnetSkill([FromBody]PatchOnetSkillModel patchOnetSkillModel, Guid documentId)
        {
            logger.LogInformation($"{PatchOnetSkillActionName} has been called");

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
                logger.LogError($"{PatchOnetSkillActionName}: Error while patching Related Skill content for Job Profile with Id: {patchOnetSkillModel.JobProfileId} for {patchOnetSkillModel.Title} ");
            }

            return new StatusCodeResult((int)response);
        }

        [HttpPatch]
        [Route("segment/{documentId}/skillsMatrix")]
        public async Task<IActionResult> PatchSkillsMatrix([FromBody]PatchContextualisedModel patchContextualisedModel, Guid documentId)
        {
            logger.LogInformation($"{PatchSkillsMatrixActionName} has been called");

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
                logger.LogError($"{PatchSkillsMatrixActionName}: Error while patching Skills Matrix content for Job Profile with Id: {patchContextualisedModel.JobProfileId} for {patchContextualisedModel.Description} ");
            }

            return new StatusCodeResult((int)response);
        }

        [HttpPatch]
        [Route("segment/{documentId}/restriction")]
        public async Task<IActionResult> PatchRestriction([FromBody]PatchRestrictionModel patchRestrictionModel, Guid documentId)
        {
            logger.LogInformation($"{PatchRestrictionActionName} has been called");

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
                logger.LogError($"{PatchRestrictionActionName}: Error while patching Skills Matrix content for Job Profile with Id: {patchRestrictionModel.JobProfileId} for {patchRestrictionModel.Title} ");
            }

            return new StatusCodeResult((int)response);
        }

        [HttpDelete]
        [Route("{controller}/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            logger.LogInformation($"{DeleteActionName} has been called");

            var isDeleted = await skillSegmentService.DeleteAsync(documentId).ConfigureAwait(false);
            if (isDeleted)
            {
                logger.LogInformation($"{DeleteActionName} has deleted content for document Id: {documentId}");
                return Ok();
            }
            else
            {
                logger.LogWarning($"{DeleteActionName} has returned no content for: {documentId}");
                return NotFound();
            }
        }
    }
}