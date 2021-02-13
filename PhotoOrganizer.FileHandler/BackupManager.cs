﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace PhotoOrganizer.FileHandler
{
    public class BackupManager
    {
        private Dictionary<Type, List<Dictionary<string, Tuple<Type, object>>>> tableContent;

        public void ReadAllTable<T>(T dbContext)
        {
            tableContent = new Dictionary<Type, List<Dictionary<string, Tuple<Type, object>>>>();

            Type contextType = typeof(T);
            PropertyInfo[] properties = contextType.GetProperties();

            foreach (var property in properties)
            {
                MethodInfo[] methInfos = property.GetAccessors();
                foreach (var propertyMethod in methInfos)
                {
                    // TODO: filter to DbSet as a string
                    if (propertyMethod.ReturnType != typeof(void))
                    {
                        var invokeResult = propertyMethod.Invoke(dbContext, new object[] { });
                        var propertyType = invokeResult.GetType();
                        if (propertyType.Name.Contains("DbSet"))
                        {
                            var extensionMethod = typeof(QueryableExtensions).GetMethods().
                                Where(x => x.Name == "ToListAsync").FirstOrDefault(x => !x.IsGenericMethod);

                            dynamic result = extensionMethod.Invoke(null, new[] { invokeResult });
                            var list = result.Result;
                            ReadTableValues(list);
                        }
                    }
                }
            }
        }

        private void ReadTableValues<T>(List<T> table)
        {
            if (table == null || table.Count == 0)
            {
                return;
            }

            var listOfOneType = new List<Dictionary<string, Tuple<Type, object>>>();
            Type itemType = null;
            foreach (var item in table)
            {
                var entityContent = new Dictionary<string, Tuple<Type, object>>();
                itemType = item.GetType();
                var props = itemType.GetProperties();
                //it doesn't know the type must be casted
                foreach (var prop in props)
                {
                    entityContent.Add(prop.Name, new Tuple<Type, object>(prop.PropertyType, item.GetType().GetProperty(prop.Name).GetValue(item)));
                }
                listOfOneType.Add(entityContent);
            }
            tableContent.Add(itemType, listOfOneType);
        }
    }
}