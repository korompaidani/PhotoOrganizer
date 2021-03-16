using PhotoOrganizer.Common;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace PhotoOrganizer.FileHandler
{
    public static class BitmapExtension
    {
        public static Bitmap SetMetaValue(this Bitmap sourceBitmap, MetaProperty property, string value)
        {
            PropertyItem prop = sourceBitmap.PropertyItems[0];
            int iLen = value.Length + 1;
            byte[] bTxt = new Byte[iLen];
            for (int i = 0; i < iLen - 1; i++)
                bTxt[i] = (byte)value[i];
            bTxt[iLen - 1] = 0x00;
            prop.Id = (int)property;
            prop.Type = 2;
            prop.Value = bTxt;
            prop.Len = iLen;
            sourceBitmap.SetPropertyItem(prop);
            return sourceBitmap;
        }

        public static string GetMetaValue(this Bitmap sourceBitmap, MetaProperty property)
        {
            PropertyItem[] propItems = sourceBitmap.PropertyItems;
            var prop = propItems.FirstOrDefault(p => p.Id == (int)property);
            if (prop != null)
            {
                return Encoding.UTF8.GetString(prop.Value);
            }
            else
            {
                return null;
            }
        }
    }
}
