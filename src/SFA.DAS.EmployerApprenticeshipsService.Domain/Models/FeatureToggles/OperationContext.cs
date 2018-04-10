﻿using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
    public static class FeatureHandlerResults
    {
        public static readonly Task<bool> FeatureDisabledTask = Task.FromResult(false);
        public static readonly Task<bool> FeatureEnabledTask = Task.FromResult(true);
    }
}
