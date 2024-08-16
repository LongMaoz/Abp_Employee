using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
    /// <summary>
    /// 七牛云空间要求文件格式
    /// </summary>
   public class FileUploadResult
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string Bucket { get; set; }
        /// <summary>
        /// 键值
        /// </summary>
        public string Key { get; set; }
    }
}
