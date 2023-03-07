using PhotoOrganizer.Common;
using System;
using System.Collections.Generic;

namespace PhotoOrganizer.FileHandler.MetaConverters
{
    public class ConverterFactory : IConverterFactory
    {
        private Dictionary<MetaProperty, Lazy<IMetaConverter>> ConverterMap = new Dictionary<MetaProperty, Lazy<IMetaConverter>>
        {
            { MetaProperty.Title, new Lazy<IMetaConverter>(() => new TitleConverter()) },
            { MetaProperty.Desciprion, new Lazy<IMetaConverter>(() => new DecriptionConverter()) },
            { MetaProperty.Keywords, new Lazy<IMetaConverter>(() => new KeyWordConverter()) },
            { MetaProperty.Comments, new Lazy<IMetaConverter>(() => new CommentsConverter()) },
            { MetaProperty.Latitude, new Lazy<IMetaConverter>(() => new LatitudeConverter()) },
            { MetaProperty.Longitude, new Lazy<IMetaConverter>(() => new LongitudeConverter()) },
            { MetaProperty.DateTime, new Lazy<IMetaConverter>(() => new TakenDateConverter()) }
        };

        public IMetaConverter GetMetaConverter(MetaProperty metaProperty)
        {
            Lazy<IMetaConverter> result;
            ConverterMap.TryGetValue(metaProperty, out result);
            if(result is null)
            {
                return null; 
            }
            else
            {
                return result?.Value;
            }
        }
    }
}
