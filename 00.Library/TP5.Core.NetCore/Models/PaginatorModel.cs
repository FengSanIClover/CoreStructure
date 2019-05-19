using System;
using System.Collections.Generic;
using System.Text;

namespace TP5.Core.NetCore.Models
{
    /// <summary>
    /// 分頁Model
    /// </summary>
    public class PaginatorModel
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public PaginatorModel()
        {
            PageIndex = 1;
            PageSize = 10;
            Order = "asc";
        }

        /// <summary>
        /// 頁數(取得第n頁)
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每頁筆數
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public string Sort { get; set; }
        /// <summary>
        /// 順序 asc or desc
        /// </summary>
        public string Order { get; set; }
    }
}
