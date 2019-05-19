using System;
using System.Collections.Generic;
using System.Text;
using TP5.Core.NetCore.Models;

namespace TP5.Core.NetCore.Helper
{
    /// <summary>
    /// 分頁Help
    /// </summary>
    public class PaginatorHelp
    {
        /// <summary>
        /// 取得排序字串
        /// </summary>
        /// <param name="page">PaginatorModel</param>
        /// <returns></returns>
        public static string GetSortString(PaginatorModel page)
        {
            if (string.IsNullOrEmpty(page.Sort)) return "CreatedAt asc";
            string[] orderArray = null;

            string sortString = string.Empty;
            string[] sortArray = page.Sort.Split(',');
            if (!string.IsNullOrEmpty(page.Order)) orderArray = page.Order.Split(',');
            for (int i = 0; i < sortArray.Length; i++)
            {
                string order =
                    (orderArray == null || string.IsNullOrEmpty(orderArray[i])) ? "asc" : orderArray[i];
                sortString += $"{sortArray[i]} {order},";
            }
            return sortString.Length > 1 ? sortString.TrimEnd(',') : null;
        }
    }
}
