using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentoring.D2.AOP.MergeService.Interfaces
{
    public interface IMergeManager
    {
        bool MergeFiles(IEnumerable<FileData> fileNames);
    }
}
