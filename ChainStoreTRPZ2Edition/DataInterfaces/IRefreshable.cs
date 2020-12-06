using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChainStoreTRPZ2Edition.DataInterfaces
{
    /// <summary>
    /// Interface to refresh VM's data before its rendering
    /// </summary>
    public interface IRefreshable
    {
        void RefreshData();
    }
}
