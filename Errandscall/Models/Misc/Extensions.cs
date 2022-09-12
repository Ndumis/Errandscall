using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Errandscall.Models
{
    public static class Override
    {
        //public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes)
        //{
        //    return new MvcHtmlString("");
        //}

        //public static MvcHtmlString TextBoxFor<TModel, TProperty>(
        //this HtmlHelper<TModel> helper,
        //Expression<Func<TModel, TProperty>> expression)
        //{
        //    return helper.TextBoxFor(expression, new { @class = expression.Name });
        //}
        #region MVC
        private enum Type
        {
            TextBox,
            Label
        }
        public static MvcHtmlString DtxTextBoxFor<TModel, TValue>
        (this HtmlHelper<TModel> html,
        Expression<Func<TModel, TValue>> expression,
       object htmlAttributes = null, bool readOnly = false)
        {
            string LabelText = "";
            Dictionary<string, object> dicHtmlAttributes = GetAttributes
                (html, expression, htmlAttributes, readOnly, Type.TextBox, ref LabelText);

            Dictionary<string, object> LableAttribute = GetAttributes
         (html, expression, htmlAttributes, readOnly, Type.Label, ref LabelText);

            string attributes = htmlAttributes?.ToString();


            return new MvcHtmlString(html.LabelFor(expression, LabelText, LableAttribute).ToHtmlString()
                                     + (attributes != null && attributes.Contains("datetime")
                                            ? html.TextBoxFor(expression, dicHtmlAttributes).ToHtmlString()
                                            : html.EditorFor(expression, new { htmlAttributes = dicHtmlAttributes }).ToHtmlString()
                                        )
                                     + html.ValidationMessageFor(expression, "", htmlAttributes: new { @class = "text-danger" }).ToHtmlString());

            //@Html.EditorFor(model => model.Identifier, new { htmlAttributes = new { @class = "form-control" } })

        }

        private static Dictionary<string, object> GetAttributes<TModel, TValue>
            (HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes, bool readOnly, Type type, ref string LabelText)
        {
            Dictionary<string, object> dicHtmlAttributes = new Dictionary<string, object>();
            if (htmlAttributes == null)
            {
                htmlAttributes =
                    new Dictionary<string, object>();
            }
            else
            {
                //copy htmlAttributes object to Dictionary
                foreach (var prop in htmlAttributes.GetType().GetProperties())
                {
                    dicHtmlAttributes.Add(prop.Name, prop.GetValue(htmlAttributes));
                }

            }

            ModelMetadata oModelMetadata =
                ModelMetadata.FromLambdaExpression(expression, html.ViewData);


            //if (dicHtmlAttributes.ContainsKey("htmlAttributes"))
            //{
            //    foreach (var prop in dicHtmlAttributes["htmlAttributes"].GetType().GetProperties())
            //    {
            //        dicHtmlAttributes.Add(prop.Name, prop.GetValue(htmlAttributes));
            //    }
            //}
            //else
            //{
            //    htmlAttributes =
            //       new Dictionary<string, object>();
            //}

            RouteValueDictionary dictionarytemp = new RouteValueDictionary();
            RouteValueDictionary dictionary = new RouteValueDictionary();
            if (dicHtmlAttributes.Values.Count > 0)
            {
                dictionarytemp = new RouteValueDictionary(dicHtmlAttributes.Values.First());
            }

            foreach (KeyValuePair<string, object> item in dictionarytemp)
            {
                dictionary.Add(item.Key.Replace('_', '-'), item.Value);
            }


            if (oModelMetadata == null)
            {
                if (readOnly)
                {
                    if (dictionary.ContainsKey("readonly") == false)
                    {
                        dictionary.Add("readonly", "read-only");
                    }
                }
            }
            else
            {


                if (dictionary.ContainsKey("placeholder") == false)
                {
                    string strHtmlFieldName =
                     ExpressionHelper.GetExpressionText(expression);

                    string strLabelText =
                        oModelMetadata.DisplayName ??
                      ToSentenceCase(oModelMetadata.PropertyName) ??
                        strHtmlFieldName.Split('.').Last();

                    if (string.IsNullOrEmpty(strLabelText) == false)
                    {
                        dictionary.Add("placeholder", strLabelText);
                    }


                }
                else
                {

                }

                if ((readOnly) || (oModelMetadata.IsReadOnly))
                {
                    if (dictionary.ContainsKey("readonly") == false)
                    {
                        dictionary.Add("readonly", "read-only");
                    }
                }
            }
            if (!dictionary.ContainsKey("class"))
            {
                dictionary.Add("class", "form-control");
            }
            else
            {
                if (!dictionary["class"].ToString().Contains("form-control"))
                {
                    dictionary["class"] += " form-control";
                }

            }

            if (type == Type.Label)
            {
                if (dictionary.ContainsKey("labelText"))
                {
                    LabelText = dictionary["labelText"].ToString();
                }
                else
                {
                    LabelText = dictionary["placeholder"].ToString();
                }
            }


            if (expression.Body is MemberExpression oMemberExpression)
            {
                StringLengthAttribute oStringLengthAttribute =
                    oMemberExpression.Member.GetCustomAttributes
                    (typeof(StringLengthAttribute), false)
                    .FirstOrDefault() as StringLengthAttribute;

                if (oStringLengthAttribute != null)
                {
                    if (dictionary.ContainsKey("maxlength") == false)
                    {
                        dictionary.Add("maxlength", oStringLengthAttribute.MaximumLength);
                    }
                }
            }

            dicHtmlAttributes.Clear();
            foreach (var item in dictionary)
            {
                string value = item.Value.ToString().Replace('_', '-');
                if (type == Type.Label)
                {
                    if (item.Value.ToString().Contains("form-control"))
                    {
                        value = item.Value.ToString().Replace("form-control", "");
                    }

                }
                //LableAttribute["class"] = LableAttribute["class"].ToString().Replace("form-control", "");
                dicHtmlAttributes.Add(item.Key, value);
            }

            if (!dicHtmlAttributes.ContainsKey("id"))
            {
                dicHtmlAttributes.Add("id", oModelMetadata.PropertyName ?? "");
            }

            if (type == Type.Label)
            {
                if (dicHtmlAttributes.ContainsKey("id"))
                {
                    dicHtmlAttributes["id"] = dicHtmlAttributes["id"].ToString() + "_Label";
                }
            }

            return dicHtmlAttributes;
        }

        public static MvcHtmlString AntiForgeryTokenForAjaxPost(this HtmlHelper helper)
        {
            var antiForgeryInputTag = helper.AntiForgeryToken().ToString();
            // Above gets the following: <input name="__RequestVerificationToken" type="hidden" value="PnQE7R0MIBBAzC7SqtVvwrJpGbRvPgzWHo5dSyoSaZoabRjf9pCyzjujYBU_qKDJmwIOiPRDwBV1TNVdXFVgzAvN9_l2yt9-nf4Owif0qIDz7WRAmydVPIm6_pmJAI--wvvFQO7g0VvoFArFtAR2v6Ch1wmXCZ89v0-lNOGZLZc1" />
            var removedStart = antiForgeryInputTag.Replace(@"<input name=""__RequestVerificationToken"" type=""hidden"" value=""", "");
            var tokenValue = removedStart.Replace(@""" />", "");
            if (antiForgeryInputTag == removedStart || removedStart == tokenValue)
                throw new InvalidOperationException("Oops! The Html.AntiForgeryToken() method seems to return something I did not expect.");
            return new MvcHtmlString(string.Format(@"{0}:""{1}""", "__RequestVerificationToken", tokenValue));
        }


        #endregion

        #region Converters
        /// <summary>
        /// Coneverts to <see cref="int"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns></returns>
        public static int ToInt(this object Objects)
        {
            int Result = 0;
            if (Objects == null)
            {
                return Result;
            }

            if (int.TryParse(Objects.ToString(), out Result))
            {
                return Result;
            }

            return Result;

        }

        /// <summary>
        /// Converts to nullable <see cref="int"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns></returns>
        public static int? ToIntNull(this object Objects)
        {
            int? Result = null;
            int OutResult;
            if (Objects == null)
            {
                return Result;
            }

            if (int.TryParse(Objects.ToString(), out OutResult))
            {
                return OutResult;
            }

            return Result;

        }

        /// <summary>
        /// Converts to <see cref="DateTime"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns><see cref="DateTime"/></returns>
        public static DateTime ToDateTime(this object Objects)
        {
            DateTime Result = new DateTime();
            if (Objects == null)
            {
                return Result;
            }

            if (DateTime.TryParse(Objects.ToString(), out Result))
            {
                return Result;
            }

            return Result;

        }
        /// <summary>
        /// Converts to nullable <see cref="DateTime"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns><see cref="DateTime"/>?</returns>
        public static DateTime? ToDateTimeNull(this object Objects)
        {
            DateTime? Result = null;
            DateTime OutResult;
            if (Objects == null)
            {
                return Result;
            }

            if (DateTime.TryParse(Objects.ToString(), out OutResult))
            {
                return OutResult;
            }

            return Result;

        }
        /// <summary>
        /// Converts to <see cref="decimal"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns><see cref="decimal"/></returns>
        public static decimal ToDecimal(this object Objects)
        {
            decimal Result = new decimal();
            if (Objects == null)
            {
                return Result;
            }

            if (decimal.TryParse(Objects.ToString(), out Result))
            {
                return Result;
            }

            return Result;

        }

        /// <summary>
        /// Converts to nullable <see cref="decimal"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns><see cref="decimal"/>?</returns>
        public static decimal? ToDecimalNull(this object Objects)
        {
            decimal? Result = null;
            decimal OutResult;
            if (Objects == null)
            {
                return Result;
            }

            if (decimal.TryParse(Objects.ToString(), out OutResult))
            {
                return OutResult;
            }

            return Result;

        }


        /// <summary>
        /// Converts to <see cref="double"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns><see cref="double"/></returns>
        public static double ToDouble(this object Objects)
        {
            double Result = new double();
            if (Objects == null)
            {
                return Result;
            }

            if (double.TryParse(Objects.ToString(), out Result))
            {
                return Result;
            }

            return Result;

        }

        /// <summary>
        /// Converts to nullable <see cref="double"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns><see cref="double"/>?</returns>
        public static double? ToDoubleNull(this object Objects)
        {
            double? Result = null;
            double OutResult;
            if (Objects == null)
            {
                return Result;
            }

            if (double.TryParse(Objects.ToString(), out OutResult))
            {
                return OutResult;
            }

            return Result;

        }

        /// <summary>
        /// Coneverts to <see cref="float"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns></returns>
        public static float ToFloat(this object Objects)
        {
            float Result = 0;
            if (Objects == null)
            {
                return Result;
            }

            if (float.TryParse(Objects.ToString(), out Result))
            {
                return Result;
            }

            return Result;

        }

        /// <summary>
        /// Converts to nullable <see cref="float"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns></returns>
        public static float? ToFloatNull(this object Objects)
        {
            float? Result = null;
            float OutResult;
            if (Objects == null)
            {
                return Result;
            }

            if (float.TryParse(Objects.ToString(), out OutResult))
            {
                return OutResult;
            }

            return Result;

        }


        /// <summary>
        /// Coneverts to <see cref="long"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns></returns>
        public static long ToLong(this object Objects)
        {
            long Result = 0;
            if (Objects == null)
            {
                return Result;
            }

            if (long.TryParse(Objects.ToString(), out Result))
            {
                return Result;
            }

            return Result;

        }

        /// <summary>
        /// Converts to nullable <see cref="long"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns></returns>
        public static long? ToLongNull(this object Objects)
        {
            long? Result = null;
            long OutResult;
            if (Objects == null)
            {
                return Result;
            }

            if (long.TryParse(Objects.ToString(), out OutResult))
            {
                return OutResult;
            }

            return Result;

        }
        /// <summary>
        /// Converts to byte Array, supports <see cref="MemoryStream"/> and implicit conversion
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns><see cref="byte"/>[]</returns>
        public static byte[] ToByteArray(this object Objects)
        {
            if (Objects is System.IO.MemoryStream)
            {
                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = (Objects as MemoryStream).Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }


            byte[] Result = null;
            if (Objects == null || Objects == DBNull.Value)
            {
                return Result;
            }

            return (byte[])Objects;
        }

        /// <summary>
        /// Converts to boolean, returns false if null
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns><see cref="bool"/></returns>
        public static bool ToBool(this object Objects)
        {
            bool Result = false;
            if (Objects == null)
            {
                return Result;
            }

            if (Objects.ToString() == "1" || Objects.ToString().ToLower() == "true")
            {
                return true;
            }

            if (bool.TryParse(Objects.ToString(), out Result))
            {
                return Result;
            }

            return Result;

        }


        /// <summary>
        /// Coneverts to <see cref="Guid"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns></returns>
        public static Guid ToGuid(this object Objects)
        {
            Guid Result = default(Guid);
            if (Objects == null)
            {
                return Result;
            }

            if (Guid.TryParse(Objects.ToString(), out Result))
            {
                return Result;
            }

            return Result;

        }

        /// <summary>
        /// Converts to nullable <see cref="Guid"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns></returns>
        public static Guid? ToGuidNull(this object Objects)
        {
            Guid? Result = null;
            Guid OutResult;
            if (Objects == null)
            {
                return Result;
            }

            if (Guid.TryParse(Objects.ToString(), out OutResult))
            {
                return OutResult;
            }

            return Result;

        }

        /// <summary>
        /// Converts to a serialized <see cref="string"/>
        /// </summary>
        /// <param name="Objects"><see cref="object"/> to serialize</param>
        /// <returns><see cref="string"/></returns>
        public static string ToJson(this object Objects)
        {
            string Result = null;
            if (Objects == null)
            {
                return Result;
            }

            Result = Newtonsoft.Json.JsonConvert.SerializeObject(Objects, new Newtonsoft.Json.Formatting()
            {

            }, new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return Result;

        }

        /// <summary>
        /// Converts from a <see cref="string"/> from a selected type
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> to deserialized</typeparam>
        /// <param name="Objects"><see cref="string"/> to be serialzed</param>
        /// <returns><typeparamref name="T"/>, if error occurs returns defualt of <typeparamref name="T"/></returns>
        public static T FromJson<T>(this string Objects)
        {
            T Result = default(T);
            if (Objects == null)
            {
                return Result;
            }

            try
            {
                Result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Objects);
            }
            catch (Exception)
            {
                return Result;
            }

            return Result;

        }

        /// <summary>
        /// Returns an array to the limit set by the count
        /// </summary>
        /// <typeparam name="T">Array <see cref="Type"/></typeparam>
        /// <param name="EnumarableClass"><see cref="Type"/></param>
        /// <param name="count">The <paramref name="count"/> max that is needed</param>
        /// <returns><see cref="Array"/> of <typeparamref name="T"/></returns>
        public static T[] ToFixedArray<T>(this T[] EnumarableClass, int count)
        {
            var Array = new T[count];
            for (int i = 0; i < count; i++)
            {
                if (i <= EnumarableClass.Count() - 1)
                {
                    Array[i] = EnumarableClass[i];

                }
                else
                    break;
            }
            return Array.ToArray();
        }
        /// <summary>
        /// Converts To Timestamp
        /// </summary>
        /// <param name="date"></param>
        /// <returns><see cref="UInt32"/></returns>
        public static UInt32 ToTimestamp(this DateTime date)
        {
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // FISF-934 Set 0 problem when converted posix time
            //DateTime currDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
            DateTime currDate = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, DateTimeKind.Utc);
            // FISF-934 Set 0 problem when converted posix time
            var seconds = currDate.Subtract(baseDate).TotalSeconds;

            return Convert.ToUInt32(seconds);

        }

        /// <summary>
        /// Converts from unix timestamp
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns><see cref="DateTime"/></returns>
        public static DateTime FromUnixTimestamp(this uint timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);

        }

        /// <summary>
        /// Converts an object to an enum type 
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="Objects"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this object Objects)
        {
            T Value = (T)Enum.Parse(typeof(T), Objects.ToString());

            return Value;

        }

        /// <summary>
        /// Allows you to set the message for the for an exception using string format 
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <param name="message">The message to set</param>
        /// <param name="vs">The format for the string.Format</param>
        /// <returns></returns>
        public static Exception SetException(this Exception ex, string message, params object[] vs)
        {

            return new Exception(string.Format(message, vs));

        }
        /// <summary>
        /// Allows you to set the message for the for an exception  
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <param name="message">The message to set</param>
        /// <returns></returns>
        public static Exception SetException(this Exception ex, string message)
        {
            return new Exception(message);

        }

        /// <summary>
        /// Checks if the <see cref="DataTable"/> has rows
        /// </summary>
        /// <param name="Objects">The <see cref="DataTable"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool HasRows(this DataTable Objects) => Objects != null && Objects.Rows != null && Objects.Rows.Count > 0;
        /// <summary>
        /// Converts date to neaMetrics standard date string
        /// </summary>
        /// <param name="Date">Date that needs to be converted</param>
        /// <returns>neaMetrics standard 14 digit string</returns>
        public static string ToNeaMetricsDate(this DateTime Date)
        {
            return Date.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// Converts from <see cref="neaMetrics"/> 14 digit standard string
        /// </summary>
        /// <param name="Date"><see cref="string"/> that needs to be converted</param>
        /// <returns><see cref="DateTime"/> converted from <see cref="string"/></returns>
        public static DateTime? FromNeaMetricsDate(this string Date)
        {
            DateTime? Result = null;
            DateTime OutResult;
            if (Date == null)
            {
                return Result;
            }

            if (DateTime.TryParseExact(Date.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out OutResult))
            {
                return OutResult;
            }

            return Result;
        }

        public static string FroNeaMetricsDateString(this decimal Date)
        {
            string Result = null;
            DateTime OutResult;


            if (DateTime.TryParseExact(Date.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out OutResult))
            {
                return OutResult.ToString("dd-MM-yyyy HH:mm");
            }

            return Result;
        }

        public static string FroNeaMetricsDateString(this decimal? Date)
        {
            string Result = null;
            DateTime OutResult;
            if (Date == null)
            {
                return Result;
            }

            if (DateTime.TryParseExact(Date.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out OutResult))
            {
                return OutResult.ToString("dd-MM-yyyy HH:mm");
            }

            return Result;
        }

        public static DateTime FromNeaMetricsDate(this decimal addedOnDateTime)
        {
            DateTime.TryParseExact(addedOnDateTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo
                          .InvariantCulture,
                          System.Globalization.DateTimeStyles.None, out var outputDate);

            return outputDate;
        }

        #endregion

        #region StringManipulation
        /// <summary>
        /// Checks of the string is empty or null
        /// </summary>
        /// <param name="Objects">string</param>
        /// <returns>bool</returns>
        public static bool IsNullOrEmpty(this string Objects) => string.IsNullOrEmpty(Objects);
        /// <summary>
        /// Gets string from byte[]
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns><see cref="byte"/>[]</returns>
        public static string GetString(this byte[] Objects) => Objects == null ? "" : Encoding.UTF8.GetString(Objects).Replace("\0", "").Trim();


        /// <summary>
        /// Gets bytes from <see cref="string"/>
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns><see cref="byte"/>[]</returns>
        public static byte[] GetBytes(this string Objects) => Encoding.UTF8.GetBytes(Objects.Replace("\0", "").Trim());

        public static string ReplaceMultiple(this string Objects, string ReplaceWith, params string[] ReplaceWords)
        {
            if (Objects == null)
                return null;
            foreach (var item in ReplaceWords)
            {
                Objects = Objects.Replace(item, ReplaceWith);
            }
            return Objects;
        }

        public static string ToSentenceCase(string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + m.Value[1]);
        }

        public static byte[] ExtractBytesFromBase64(this string base64String)
        {
            try
            {
                base64String = base64String.Replace(string.Format("data:image/png;base64,",
                    nameof(base64String)), string.Empty);

                base64String = base64String.Replace(string.Format("data:image/jpeg;base64,",
                    nameof(base64String)), string.Empty);

                return
                    Convert.FromBase64String(base64String);
            }
            catch
            {
                return new byte[0];
            }
        }

        #endregion
    }
}