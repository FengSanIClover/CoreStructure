using System;
using System.Collections.Generic;
using System.Text;

namespace TP5.Core.NetCore.Models
{
    /// <summary>
    /// 分頁結果
    /// </summary>
    public class PaginatorResult
    {
        /// <summary>
        /// 資料總筆數
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// 分頁結果
        /// </summary>
        public object data { get; set; }
    }
}
