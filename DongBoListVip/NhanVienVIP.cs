using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DongBoListVip
{
    class NhanVienVIP
    {
        public static Log_Sytems logs = new Log_Sytems();
        public static MySqlDataHelper db = new MySqlDataHelper();
        public static SqlDataHelper dbSql = new SqlDataHelper();

        public static DataTable DanhSachNhanVienVIPSql()
        {
            DataTable dt = new DataTable();
            try
            {
                dt = dbSql.ExecuteDataSet("SELECT * FROM vip_list").Tables[0];

                return dt;
            }
            catch (Exception ex)
            {
                logs.ErrorLog(ex.Message, ex.StackTrace);
                return dt;
            }
        }

        public static DataTable DanhSachNhanVienVIPMySql()
        {
            DataTable dt = new DataTable();
            try
            {
                dt = db.ExecuteDataSet("SELECT * FROM vip_list").Tables[0];

                return dt;
            }
            catch (Exception ex)
            {
                logs.ErrorLog(ex.Message, ex.StackTrace);
                return dt;
            }
        }


        public static bool InsertorUpdateNhanVienVIPMySql(string phonenumber, string name, string queue)
        {
            try
            {
                DataTable dt = db.ExecuteDataSet(string.Format("select * from vip_list where PhoneNumber = '{0}'", phonenumber)).Tables[0];
                if(dt.Rows.Count > 0)
                {
                    db.ExecuteNonQuery(string.Format("UPDATE vip_list SET PhoneNumber = '{0}', NAME = '{1}', Queue = '{2}' WHERE ID = {3}", phonenumber, name, queue, dt.Rows[0]["Id"].ToString()));
                }
                else
                {
                    db.ExecuteNonQuery(string.Format("insert into vip_list(PhoneNumber, Name, Queue) values('{0}', '{1}', '{2}')", phonenumber, name, queue));
                }

                return true;
            }
            catch (Exception ex)
            {
                logs.ErrorLog(ex.Message, ex.StackTrace);
                return false;
            }
        }

        public static bool DeleteNhanVienVIPMySql()
        {
            try
            {
                //Lấy danh sách từ SQL
                DataTable dt = DanhSachNhanVienVIPMySql();

                foreach(DataRow dr in dt.Rows)
                {
                    DataTable dt1 = dbSql.ExecuteDataSet(string.Format("select * from vip_list where PhoneNumber = '{0}', NAME = '{1}', Queue = '{2}'", dr["PhoneNumber"].ToString(), dr["Name"].ToString(), dr["Queue"].ToString())).Tables[0];
                    if(dt1.Rows.Count == 0)
                    {
                        db.ExecuteNonQuery(string.Format("DELETE FROM vip_list WHERE PhoneNumber = '{0}', NAME = '{1}', Queue = '{2}'", dr["PhoneNumber"].ToString(), dr["Name"].ToString(), dr["Queue"].ToString()));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logs.ErrorLog(ex.Message, ex.StackTrace);
                return false;
            }
        }
    }
}
