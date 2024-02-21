using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DotNetSsh
{
    internal class Profiles : Collection<DeploymentProfile>
    {
        public Profiles(IEnumerable<DeploymentProfile> profiles) : base(profiles.ToList())
        {
        }
    }
}