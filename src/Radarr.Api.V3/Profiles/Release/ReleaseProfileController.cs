using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Profiles.Releases;
using Radarr.Http;
using Radarr.Http.REST;
using Radarr.Http.REST.Attributes;

namespace Radarr.Api.V3.Profiles.Release
{
    [V3ApiController]
    public class ReleaseProfileController : RestController<ReleaseProfileResource>
    {
        private readonly IReleaseProfileService _profileService;

        public ReleaseProfileController(IReleaseProfileService profileService)
        {
            _profileService = profileService;

            SharedValidator.RuleFor(d => d).Custom((restriction, context) =>
            {
                if (restriction.MapRequired().Empty() && restriction.MapIgnored().Empty())
                {
                    context.AddFailure(nameof(ReleaseProfile.Required), "'Must contain' or 'Must not contain' is required");
                }

                if (restriction.MapRequired().Any(t => t.IsNullOrWhiteSpace()))
                {
                    context.AddFailure(nameof(ReleaseProfile.Required), "'Must contain' should not contain whitespaces or an empty string");
                }

                if (restriction.MapIgnored().Any(t => t.IsNullOrWhiteSpace()))
                {
                    context.AddFailure(nameof(ReleaseProfile.Ignored), "'Must not contain' should not contain whitespaces or an empty string");
                }
            });
        }

        [RestPostById]
        public ActionResult<ReleaseProfileResource> Create([FromBody] ReleaseProfileResource resource)
        {
            var model = resource.ToModel();
            model = _profileService.Add(model);
            return Created(model.Id);
        }

        [RestDeleteById]
        public void DeleteProfile(int id)
        {
            _profileService.Delete(id);
        }

        [RestPutById]
        public ActionResult<ReleaseProfileResource> Update([FromBody] ReleaseProfileResource resource)
        {
            var model = resource.ToModel();

            _profileService.Update(model);

            return Accepted(model.Id);
        }

        protected override ReleaseProfileResource GetResourceById(int id)
        {
            return _profileService.Get(id).ToResource();
        }

        [HttpGet]
        public List<ReleaseProfileResource> GetAll()
        {
            return _profileService.All().ToResource();
        }
    }
}
