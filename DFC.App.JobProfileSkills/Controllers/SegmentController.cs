using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
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
        private const string SaveActionName = nameof(Save);
        private const string DeleteActionName = nameof(Delete);

        private readonly ILogger<SegmentController> logger;
        private readonly IJobProfileSkillSegmentService jobProfileSkillSegmentService;
        private readonly AutoMapper.IMapper mapper;

        public SegmentController(ILogger<SegmentController> logger, IJobProfileSkillSegmentService jobProfileSkillSegmentService, AutoMapper.IMapper mapper)
        {
            this.logger = logger;
            this.jobProfileSkillSegmentService = jobProfileSkillSegmentService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/")]
        [Route("{controller}")]
        public async Task<IActionResult> Index()
        {
            logger.LogInformation($"{IndexActionName} has been called");

            var viewModel = new IndexViewModel();
            var segmentModels = await jobProfileSkillSegmentService.GetAllAsync().ConfigureAwait(false);

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

            var model = await jobProfileSkillSegmentService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

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
        [Route("{controller}/{article}/contents")]
        public async Task<IActionResult> Body(string article)
        {
            logger.LogInformation($"{BodyActionName} has been called with: {article}");

            var model = await jobProfileSkillSegmentService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

            if (model != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(model);

                logger.LogInformation($"{BodyActionName} has succeeded for: {article}");

                return this.NegotiateContentResult(viewModel, model.Data);
            }

            logger.LogWarning($"{BodyActionName} has returned no content for: {article}");

            return NoContent();
        }

        [HttpPut]
        [HttpPost]
        [Route("segment")]
        public async Task<IActionResult> Save([FromBody]JobProfileSkillSegmentModel upsertJobProfileSkillSegmentModel)
        {
            logger.LogInformation($"{SaveActionName} has been called");

            if (upsertJobProfileSkillSegmentModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await jobProfileSkillSegmentService.UpsertAsync(upsertJobProfileSkillSegmentModel).ConfigureAwait(false);

            if (response.ResponseStatusCode == HttpStatusCode.Created)
            {
                logger.LogInformation($"{SaveActionName} has created content for: {upsertJobProfileSkillSegmentModel.CanonicalName}");

                return new CreatedAtActionResult(
                    SaveActionName,
                    "Segment",
                    new { article = response.JobProfileSkillSegmentModel.CanonicalName },
                    response.JobProfileSkillSegmentModel);
            }
            else
            {
                logger.LogInformation($"{SaveActionName} has updated content for: {upsertJobProfileSkillSegmentModel.CanonicalName}");

                return new OkObjectResult(response.JobProfileSkillSegmentModel);
            }
        }

        [HttpDelete]
        [Route("{controller}/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            logger.LogInformation($"{DeleteActionName} has been called");

            var isDeleted = await jobProfileSkillSegmentService.DeleteAsync(documentId).ConfigureAwait(false);
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