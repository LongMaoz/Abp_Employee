using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
    public class UserViewModel
    {

        public int? Id { get; set; }

        /// <summary>
        /// 客户电话
        /// </summary>
        public string PhoneNumber
        {
            get;
            set;
        }
        ///// <summary>
        ///// 客户账号
        ///// </summary>
        //public string Username
        //{
        //    get;
        //    set;
        //}



        public string Email
        {
            get;
            set;
        }


        public string Wechat
        {
            get;
            set;
        }


        public string QQ
        {
            get;
            set;
        }

        public string WangWang
        {
            get;
            set;
        }


        public string DongDong
        {
            get;
            set;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 性别 male female
        /// </summary>
        public string GenderDes
        {
            get;
            set;
        }



        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthdate
        {
            get;
            set;
        }
        /// <summary>
        /// 性别 male female  不必填写 由GenderDes自动转换
        /// </summary>
        public string Gender
        {
            get
            {
                return GenderDes == "男" ? "male" : "female";
            }
            set
            {
                this.GenderDes = value == "male" ? "男" : "女";
            }
        }

        /// <summary>
        /// 头像图片(网络地址)
        /// </summary>
        public string Avatar
        {
            get;
            set;
        }

        /// <summary>
        /// 成长值 可不填
        /// </summary>

        public int? Growth
        {
            get;
            set;
        }

        /// <summary>
        /// Grade 会员等级  可不填
        /// </summary>
        public long? Grade
        {
            get;
            set;
        }

        /// <summary>
        /// Rank    可不填
        /// </summary>
        public long? Rank
        {
            get;
            set;
        }

        /// <summary>
        /// 是否认证  可不填
        /// </summary>
        public bool? IsCertified
        {
            get;
            set;
        }

        /// <summary>
        /// IsCorporate  可不填
        /// </summary>
        public bool? IsCorporate
        {
            get;
            set;
        }

        /// <summary>
        /// IsBlacklisted   可不填
        /// </summary>
        public bool? IsBlacklisted
        {
            get;
            set;
        }

        /// <summary>
        /// 年龄  不必填写 由BirthDate自动计算
        /// </summary>
        public int? Age
        {
            get
            {
                if (this.Birthdate != null) return DateTime.Now.Year - this.Birthdate.Value.Year;
                else return null;
            }
        }


    }

    /// <summary>
    /// 人员信息
    /// </summary>
    public class UserInfor
    {
        /// <summary>
        /// 编号ID
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }
   
    /// <summary>
    /// 用户详情信息
    /// </summary>
    public class UserDetail
    {
        // {"sub":4,"gender":"unknown","name":"刘小青","picture":null,"profile":null,"updated_at":"2018-12-05T09:22:18.000Z"}
        public int Sub { get; set; }
        public string Gender { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string Profile { get; set; }

    }
}
