using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Helpers
{
    public partial class FileHelper
    {
        public class INI
        {
            /// Get and set the values in the initial file.
            /// Updated date: 2020-11-04

            //public static TLog Logger { get; set; }
            //public static Exception LastException { get; set; }

            /// Get or set the parameter in the buffer from the INI file.
            /// Return value = value of the key if Get action. String buffer if Set action.
            /// bAction = false if get the parameter from buffer. True if set the parameter to buffer
            /// sBuffer = string storing the content of INI file
            /// sKey = key
            /// (optional) sSection = section
            /// (optional) sValue = default value if Get action. If cannot find the key, the function will return this default value.
            ///                     If Set action, it is a value that to be set in INI file.
            /// (optional) sComment = string of comment. Content after the 'sComment' string in the line will be considered as comment. If it is equal to Nothing or "", the string after the "=" character will be considered as the value.
            /// (optional) bMultiValue = Does it contain multiple values? False if it is single value. Ture if it contains multiple values. If it is multiple values, the multiple values will be concantenated with the delimiter 'sMultiValueDelimiter' in the 'sValue'.
            /// (optional) sMultiValueDelimiter = Delimiter to separate the multiple values.
            /// https://stackoverflow.com/questions/1547476/easiest-way-to-split-a-string-on-newlines-in-net
            public static string DoIniParaInBuffer(bool bAction, string sBuffer, string sKey) { return DoIniParaInBuffer(bAction, sBuffer, sKey, ""); }
            public static string DoIniParaInBuffer(bool bAction, string sBuffer, string sKey, string sSection) { return DoIniParaInBuffer(bAction, sBuffer, sKey, sSection, ""); }
            public static string DoIniParaInBuffer(bool bAction, string sBuffer, string sKey, string sSection, string sValue) { return DoIniParaInBuffer(bAction, sBuffer, sKey, sSection, sValue, ";"); }
            public static string DoIniParaInBuffer(bool bAction, string sBuffer, string sKey, string sSection, string sValue, string sComment) { return DoIniParaInBuffer(bAction, sBuffer, sKey, sSection, sValue, sComment, false); }
            public static string DoIniParaInBuffer(bool bAction, string sBuffer, string sKey, string sSection, string sValue, string sComment, bool bMultiValue) { return DoIniParaInBuffer(bAction, sBuffer, sKey, sSection, sValue, sComment, bMultiValue, ";"); }
            public static string DoIniParaInBuffer(bool bAction, string sBuffer, string sKey, string sSection, string sValue, string sComment, bool bMultiValue, string sMultiValueDelimiter)
            {
                string sReturn, s, s2, sKey2, sSection2;
                bool bLoopLine, bSection, bSectionMatch, bContinueRemaining;
                int iLine, iComment, iEqual, iValue, iLastLineSection, iMultiValue;
                string[] sArr;
                string[] sArrMultiValue;
                char[] cArrayTrim = { (char)9, ' ', (char)10, (char)13 };
                if (bAction) { sReturn = sBuffer; }/// set the buffer as the return value.
                else { sReturn = bMultiValue ? "" : sValue; }/// set the default value as the return value if it is not multiple value.
                try
                {
                    if (string.IsNullOrEmpty(sBuffer) || string.IsNullOrEmpty(sKey)) return sReturn;
                    sKey2 = sKey.Trim(cArrayTrim).ToLower();
                    if (string.IsNullOrEmpty(sKey2)) return sReturn;
                    /// section
                    if (string.IsNullOrEmpty(sSection)) sSection2 = "";
                    else sSection2 = sSection.Trim(cArrayTrim).ToLower();
                    bSection = string.IsNullOrEmpty(sSection2) ? false : true;/// indicate if it is necessary to search section.
                                                                              /// comment
                    if (string.IsNullOrEmpty(sComment)) sComment = string.Empty;/// set to Empty instead of "".
                                                                                /// if multiple values
                    iMultiValue = 0;
                    sArrMultiValue = sValue.Split(new string[] { sMultiValueDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                    /// loop every line.
                    sArr = sBuffer.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    bLoopLine = true; iLine = 0; bSectionMatch = false; iLastLineSection = -1; bContinueRemaining = true;
                    while (bLoopLine && (iLine < sArr.Length))
                    {
                        /// Get line.
                        s = sArr[iLine].ToString();
                        if (!string.IsNullOrEmpty(s))
                        {
                            if (string.IsNullOrEmpty(sComment)) iComment = -1;
                            else iComment = s.IndexOf(sComment);/// position of comment.

                            /// Search section.
                            if (bSection)/// if need to find section.
                            {
                                /// Get line without comment.
                                if (iComment > -1) s2 = s.Substring(0, iComment).Trim(cArrayTrim);
                                else s2 = s.Trim(cArrayTrim);
                                if (s2.Substring(0, 1).Equals("[") && s2.Substring(s2.Length - 1, 1).Equals("]"))
                                {
                                    s2 = s2.Substring(1, s2.Length - 2);
                                    if (s2.Trim(cArrayTrim).ToLower().Equals(sSection2))
                                    {
                                        /// if find the match section, go to next line immediately and test if the next section reaches.
                                        if (bSectionMatch)
                                        {
                                            /// for bAction=True. Store the (current-1) line number.
                                            if (bAction && (iLastLineSection < 0)) { iLastLineSection = iLine - 1; }
                                        }
                                        else bSectionMatch = true;
                                        // iLine += 1;
                                        /////////////////////// Continue While
                                        bContinueRemaining = false;/// Set to NOT continue running the remaining and go directly to next loop.
                                    }
                                    else
                                    {
                                        /// if find another section
                                        if (bSectionMatch)
                                        {
                                            bSectionMatch = false;/// set it to false so that stop to search key.
                                                                  /// for bAction=True. Store the (current-1) line number.
                                            if (bAction && (iLastLineSection < 0)) iLastLineSection = iLine - 1;
                                        }
                                    }
                                }
                            }

                            if (bContinueRemaining)
                            {
                                /// Search key.
                                if ((bSection == false) || bSectionMatch)// if ((bSection == false) || (bSection && bSectionMatch))
                                {
                                    if (iComment > -1) iEqual = s.IndexOf("=", 0, iComment);
                                    else iEqual = s.IndexOf("=");
                                    if (iEqual > 0)
                                    {
                                        /// If match the key.
                                        if (s.Substring(0, iEqual).Trim(cArrayTrim).ToLower().Equals(sKey2))
                                        {
                                            iValue = iEqual + 1;/// position of the start of value.
                                            if (bAction)
                                            {
                                                s2 = s.Substring(0, iValue);
                                                if (bMultiValue)
                                                {
                                                    if (iMultiValue < sArrMultiValue.Length) s2 += sArrMultiValue[iMultiValue].ToString();
                                                    else
                                                    {
                                                        /// do nothing, in order to let it loops continuously to find out the other lines that may contains same key.
                                                    }
                                                    iMultiValue += 1;
                                                    iLastLineSection = iLine;/// for bAction=True. Store the current line number.
                                                }
                                                else
                                                {
                                                    s2 += sValue;
                                                    bLoopLine = false;/// stop looping so that the function returns the first value only.
                                                }
                                                if (iComment > -1) s2 += s.Substring(iComment);/// comment.
                                                sArr[iLine] = s2;
                                                sReturn = string.Join(Environment.NewLine, sArr);
                                            }
                                            else
                                            {
                                                /// bAction = False, get value(s).
                                                if (iValue < s.Length)
                                                {
                                                    /// return value.
                                                    if (iComment > -1) s2 = s.Substring(iValue, iComment - iValue);/// iComment must be larger than or equal to iValue, according to the logic of the above lines.
                                                    else s2 = s.Substring(iValue);
                                                    /// multiple values?
                                                    if (bMultiValue) sReturn += s2 + sMultiValueDelimiter;
                                                    else
                                                    {
                                                        bLoopLine = false;/// stop looping so that the function returns the first value only.
                                                        sReturn = s2;
                                                    }
                                                }
                                                else
                                                {
                                                    /// multiple values?
                                                    if (bMultiValue) sReturn += sMultiValueDelimiter;
                                                    else
                                                    {
                                                        bLoopLine = false;/// stop looping so that the function returns the first value only.
                                                        sReturn = "";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else bContinueRemaining = true;/// Re-set this flag to true.
                        }
                        iLine += 1;
                    }
                    /// multiple values.
                    if ((bAction == false) && bMultiValue)
                    {
                        int idx = sReturn.LastIndexOf(sMultiValueDelimiter);
                        if (idx > -1) sReturn = sReturn.Remove(idx);
                    }
                    /// if cannot find the match key.
                    if (bAction && bLoopLine)
                    {
                        if (bSection && (iLastLineSection > -1))
                        {
                            s2 = "";
                            if (bMultiValue)
                            {
                                while (iMultiValue < sArrMultiValue.Length)
                                {
                                    s2 += Environment.NewLine + sKey + "=" + sArrMultiValue[iMultiValue].ToString();
                                    iMultiValue += 1;
                                }
                            }
                            else s2 = Environment.NewLine + sKey + "=" + sValue;
                            sArr[iLastLineSection] += s2;
                            sReturn = string.Join(Environment.NewLine, sArr);
                        }
                        else
                        {
                            if (bSection) sReturn += "[" + sSection + "]" + Environment.NewLine;
                            if (bMultiValue)
                            {
                                while (iMultiValue < sArrMultiValue.Length)
                                {
                                    sReturn += sKey + "=" + sArrMultiValue[iMultiValue].ToString() + Environment.NewLine;
                                    iMultiValue += 1;
                                }
                            }
                            else sReturn += sKey + "=" + sValue + Environment.NewLine;
                        }
                    }
                }
                catch (Exception ex) { Logger?.Error(ex); }
                return sReturn;
            }

            /// Get the bool value from a string value of the initial file.
            /// Return value = bool value
            /// sString = input string
            public static bool GetBool(string input)
            {
                try
                {
                    switch (input?.Trim().ToUpper())
                    {
                        case "1":
                        case "YES":
                        case "Y":
                        case "TRUE":
                        case "T":
                        case "ON":
                            return true;
                        default:
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error(ex);
                    return false;
                }
            }

            public static int? GetInt(string input)
            {
                try
                {
                    if (int.TryParse(input, out int i)) return i;
                    else
                    {
                        Logger?.Error("Fail to parse to integer. String = {0}", input);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error(ex);
                    return null;
                }
            }

            public static int? GetInt(string input, int lowerBound)
            {
                int? i = GetInt(input);
                if (i.HasValue && i.GetValueOrDefault() < lowerBound) return lowerBound;
                return i;
            }
        }
    }
}
