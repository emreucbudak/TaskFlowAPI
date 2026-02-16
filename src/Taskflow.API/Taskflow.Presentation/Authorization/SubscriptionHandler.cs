using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TaskFlow.BuildingBlocks.Interfaces;
using Identity.Domain.Entities;

namespace Taskflow.Presentation.Authorization
{
    public class SubscriptionHandler : AuthorizationHandler<SubscriptionRequirement>
    {
        private readonly ISubscriptionChecker _subscriptionChecker;
        private readonly UserManager<User> _userManager;

        public SubscriptionHandler(ISubscriptionChecker subscriptionChecker, UserManager<User> userManager)
        {
            _subscriptionChecker = subscriptionChecker;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SubscriptionRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Fail();
                return;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                context.Fail();
                return;
            }


            var isSubscribed = await _subscriptionChecker.CheckSubscriptionStatusAsync(user.CompanyId);

            if (isSubscribed)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}