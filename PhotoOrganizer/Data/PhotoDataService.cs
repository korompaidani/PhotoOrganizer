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
            yield return new Photo { FileName = "SAN_20190314_072446_Canon_EOS_450D_IMG_1606.JPG", FullPath = @".\..\..\Resources\TestResources\SAN_20190314_072446_Canon_EOS_450D_IMG_1606.JPG" };
            yield return new Photo { FileName = "SAN_20190314_080308_Canon_EOS_450D_IMG_1645.JPG", FullPath = @".\..\..\Resources\TestResources\SAN_20190314_080308_Canon_EOS_450D_IMG_1645.JPG" };
            yield return new Photo { FileName = "SAN_20190314_141615_Canon_EOS_450D_IMG_1866.JPG", FullPath = @".\..\..\Resources\TestResources\SAN_20190314_141615_Canon_EOS_450D_IMG_1866.JPG" };
            yield return new Photo { FileName = "SAN_20190315_100244_Canon_EOS_450D_IMG_2123.JPG", FullPath = @".\..\..\Resources\TestResources\SAN_20190315_100244_Canon_EOS_450D_IMG_2123.JPG" };
            yield return new Photo { FileName = "SAN_20190315_121052_Canon_EOS_450D_IMG_2153.JPG", FullPath = @".\..\..\Resources\TestResources\SAN_20190315_121052_Canon_EOS_450D_IMG_2153.JPG" };
        }
    }
}
