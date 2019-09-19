using DFC.App.JobProfileSkills.Data.Contracts;
using DFC.App.JobProfileSkills.Data.Models;
using DFC.App.JobProfileSkills.Extensions;
using DFC.App.JobProfileSkills.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfileSkills.Controllers
{
    public class SegmentController : Controller
    {
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
            logger.LogInformation($"{nameof(Index)} has been called");

            var viewModel = new IndexViewModel();
            var segmentModels = await jobProfileSkillSegmentService.GetAllAsync().ConfigureAwait(false);

            if (segmentModels != null)
            {
                viewModel.Documents = segmentModels
                    .OrderBy(x => x.CanonicalName)
                    .Select(x => mapper.Map<IndexDocumentViewModel>(x))
                    .ToList();

                logger.LogInformation($"{nameof(Index)} has succeeded");
            }
            else
            {
                logger.LogWarning($"{nameof(Index)} has returned with no results");
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("{controller}/{article}")]
        public async Task<IActionResult> Document(string article)
        {
            logger.LogInformation($"{nameof(Document)} has been called with: {article}");

            var model = await jobProfileSkillSegmentService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

            if (model != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(model);

                logger.LogInformation($"{nameof(Document)} has succeeded for: {article}");

                return View(viewModel);
            }

            logger.LogWarning($"{nameof(Document)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("{controller}/{article}/contents")]
        public async Task<IActionResult> Body(string article)
        {
            logger.LogInformation($"{nameof(Body)} has been called with: {article}");

            var model = await jobProfileSkillSegmentService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

            if (model != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(model);

                logger.LogInformation($"{nameof(Body)} has succeeded for: {article}");

                return this.NegotiateContentResult(viewModel, model.Data);
            }

            logger.LogWarning($"{nameof(Body)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpPut]
        [HttpPost]
        [Route("{controller}")]
        public async Task<IActionResult> CreateOrUpdate([FromBody]JobProfileSkillSegmentModel createOrUpdateJobProfileSkillModel)
        {
            logger.LogInformation($"{nameof(CreateOrUpdate)} has been called");

            if (createOrUpdateJobProfileSkillModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var skillSegmentModel = await jobProfileSkillSegmentService.GetByIdAsync(createOrUpdateJobProfileSkillModel.DocumentId).ConfigureAwait(false);

            if (skillSegmentModel == null)
            {
                var createdResponse = await jobProfileSkillSegmentService.CreateAsync(createOrUpdateJobProfileSkillModel).ConfigureAwait(false);

                logger.LogInformation($"{nameof(CreateOrUpdate)} has created content for: {createOrUpdateJobProfileSkillModel.CanonicalName}");

                return new CreatedAtActionResult(nameof(Document), "Segment", new { article = createdResponse.CanonicalName }, createdResponse);
            }
            else
            {
                var updatedResponse = await jobProfileSkillSegmentService.ReplaceAsync(createOrUpdateJobProfileSkillModel).ConfigureAwait(false);

                logger.LogInformation($"{nameof(CreateOrUpdate)} has updated content for: {createOrUpdateJobProfileSkillModel.CanonicalName}");

                return new OkObjectResult(updatedResponse);
            }
        }

        [HttpDelete]
        [Route("{controller}/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            logger.LogInformation($"{nameof(Delete)} has been called");

            var skillSegmentModel = await jobProfileSkillSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (skillSegmentModel == null)
            {
                logger.LogWarning($"{nameof(Document)} has returned no content for: {documentId}");

                return NotFound();
            }

            await jobProfileSkillSegmentService.DeleteAsync(documentId, skillSegmentModel.PartitionKey).ConfigureAwait(false);

            logger.LogInformation($"{nameof(Delete)} has deleted content for: {skillSegmentModel.CanonicalName}");

            return Ok();
        }
    }
}