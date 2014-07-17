using System;
using System.Globalization;
using System.Web.Mvc;

namespace BR.ToteToToe.Web.CustomModelBinders
{
    public class DateTimeModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //var displayFormat = bindingContext.ModelMetadata.DisplayFormatString;
            var displayFormat = "dd/MM/yyyy";
            var displayFormatDateTime = "dd/MM/yyyy hh:mm:ss tt";
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (!string.IsNullOrEmpty(displayFormat) && value != null)
            {
                if (bindingContext.ModelType == typeof(DateTime?) && string.IsNullOrEmpty(value.AttemptedValue))
                    return null;

                DateTime date;

                // use the format specified in the DisplayFormat attribute to parse the date
                if (DateTime.TryParseExact(value.AttemptedValue, displayFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    return date;
                }
                if (DateTime.TryParseExact(value.AttemptedValue, displayFormatDateTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    return date;
                }
                if (DateTime.TryParse(value.AttemptedValue, out date))
                {
                    return date;
                }
                else
                {
                    bindingContext.ModelState.AddModelError(
                        bindingContext.ModelName,
                        string.Format("{0} is an invalid date format", value.AttemptedValue)
                    );
                }
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}