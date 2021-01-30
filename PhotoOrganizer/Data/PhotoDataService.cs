using PhotoOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoOrganizer
{
    public class PhotoDataService : IPhotoDataService
    {
        public IEnumerable<Photo> GetAll()
        {
            yield return new Photo { Title = "SAN_20190315_121052_Canon_EOS_450D_IMG_2153.JPG", FullPath = @".\..\..\Resources\TestResources\Photos\SAN_20190315_121052_Canon_EOS_450D_IMG_2153.JPG" };
        }
    }
}
