using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Domain.Common
{
    public static class Helper
    {
        public static PropertyInfo[] RetrievePropertiesWithFilter(object obj, BindingFlags binding)
        {
            var type = obj.GetType();

            return type.GetProperties(binding);
        }

        /// <summary>
        /// Creates a deep clone of an object. This method recursively copies the entire object hierarchy, including properties, fields, arrays, and generic lists.
        /// </summary>
        /// <param name="objSource">The object to clone.</param>
        /// <returns>A deep clone of the input object.</returns>
        public static object CloneObject(this object objSource)
        {
            try
            {
                if (objSource == null)
                {
                    // If the source object is null, return null for the clone.
                    return null;
                }

                Type typeSource = objSource.GetType();

                if (typeSource.IsValueType || typeSource == typeof(string))
                {
                    // For value types and strings, just return the object itself as they are immutable.
                    return objSource;
                }
                else if (typeSource.IsArray)
                {
                    // Handle arrays
                    Type elementType = typeSource.GetElementType();
                    Array sourceArray = (Array)objSource;
                    Array targetArray = Array.CreateInstance(elementType, sourceArray.Length);

                    for (int i = 0; i < sourceArray.Length; i++)
                    {
                        // Recursively clone elements of the array.
                        object element = sourceArray.GetValue(i);
                        targetArray.SetValue(element.CloneObject(), i);
                    }

                    return targetArray;
                }
                else if (typeSource.IsGenericType && typeSource.GetGenericTypeDefinition() == typeof(List<>))
                {
                    // Handle generic lists
                    Type listType = typeSource.GetGenericArguments()[0];
                    IList sourceList = (IList)objSource;
                    IList targetList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));

                    foreach (var listItem in sourceList)
                    {
                        // Check if the list item is itself a list and recursively clone it.
                        if (listItem.GetType().IsGenericType && listItem.GetType().GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var nestedListClone = listItem.CloneObject();
                            targetList.Add(nestedListClone);
                        }
                        else
                        {
                            // Recursively clone other types of items.
                            targetList.Add(listItem.CloneObject());
                        }
                    }

                    return targetList;
                }
                else
                {
                    // For custom reference types, create a new instance and copy properties.
                    object objTarget = Activator.CreateInstance(typeSource);

                    PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    foreach (PropertyInfo property in propertyInfo)
                    {
                        if (property.Name == "DomainEvents")
                        {
                            // Skip cloning the 'DomainEvents' property.
                            continue;
                        }

                        if (property.CanWrite)
                        {
                            // Recursively clone property values.
                            object objPropertyValue = property.GetValue(objSource, null);
                            property.SetValue(objTarget, objPropertyValue.CloneObject());
                        }
                    }

                    return objTarget;
                }
            }
            catch (Exception)
            {
                // Handle exceptions by rethrowing.
                throw;
            }
        }



        /// <summary>
        /// Converts and processes a list of custom fields from JSON data.
        /// </summary>
        /// <param name="customFieldList">The list of custom fields to process.</param>
        /// <returns>A list of processed custom fields.</returns>
        /// <exception cref="Exception">Thrown if there is an error during processing.</exception>
        /*public static List<CustomField> CreateCustomFields(List<CustomField> customFieldList)
        {
            // Create a list to store the processed custom fields
            List<CustomField> processedCustomFields = new List<CustomField>();

            // Iterate through the provided customFieldList
            foreach (var customField in customFieldList)
            {
                try
                {

                    // Get the JSON element representing the field_type
                    JsonElement jsonElement = customField.field_type;

                    // Determine the value kind of the JsonElement
                    JsonValueKind valueKind = jsonElement.ValueKind;

                    // Process the JSON element based on its value kind
                    if (valueKind == JsonValueKind.Number)
                    {
                        // Convert the field_type to a double
                        customField.field_type = Convert.ToDouble(jsonElement.GetRawText());
                        customField.field_value = Convert.ToDouble(customField.field_value);
                    }
                    else if (valueKind == JsonValueKind.String)
                    {
                        // Get the string value from the JSON element
                        string jsonString = jsonElement.GetString();

                        // Attempt to parse the string as a DateTime
                        if (DateTime.TryParse(jsonString, out DateTime dateTimeValue))
                        {
                            // Convert the field_type to a DateTime
                            customField.field_type = dateTimeValue;
                            customField.field_value = dateTimeValue;
                        }
                        else
                        {
                            // If it's not a DateTime, treat it as a string
                            customField.field_type = jsonString;
                            customField.field_value = Convert.ToString(customField.field_value);
                        }
                    }
                    else if (valueKind == JsonValueKind.True || valueKind == JsonValueKind.False)
                    {
                        // Convert the field_type to a boolean
                        customField.field_type = jsonElement.GetBoolean();
                        customField.field_value = Convert.ToBoolean(customField.field_value);
                    }
                    else
                    {
                        // Handle other value kinds as needed
                        throw new Exception("Invalid field_type: " + valueKind.ToString());
                    }

                    // Convert field_length to Int16 and field_is_required to boolean
                    customField.field_length = Convert.ToInt16(customField.field_length);
                    customField.field_is_required = Convert.ToBoolean(customField.field_is_required);

                    // Add the processed custom field to the list
                    processedCustomFields.Add(customField);
                }
                catch (Exception ex)
                {
                    // Handle exceptions if needed and rethrow with a more descriptive message
                    throw new Exception("Error while processing the JSON data for customField: " + ex.Message);
                }
            }

            // Return the list of processed custom fields
            return processedCustomFields;
        }*/
    }
}
