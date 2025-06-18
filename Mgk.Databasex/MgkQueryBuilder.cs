using System;
using System.Collections.Generic;
using System.Text;
using static Mgk.DataBasex.MgkDataBase;

namespace Mgk.DataBasex
{
    public class MgkQueryBuilder
    {
        public StringBuilder StrQuery { get; set; }
        public String Suffix { get; set; }
        public String StrQueryFinal { get; set; }
        private List<string[]> ConditionsList { get; set; }
        public int Limit { get; set; }
        public DataBaseEngineEnum DataBaseEngine { get; set; }

        public MgkQueryBuilder()
        {
            Limit = 0;
            StrQuery = new StringBuilder("");
            ConditionsList = new List<string[]>();
            DataBaseEngine = DataBaseEngineEnum.No;
        }

        public MgkQueryBuilder(string qBase)
        {
            Limit = 0;
            StrQuery = new StringBuilder(qBase);
            ConditionsList = new List<string[]>();
        }

        public void SetQueryBase(string sql)
        {
            this.StrQuery = new StringBuilder(sql);
        }

        public void Add(string condition)
        {
            this.AddAnd(condition);
        }

        public void AddAnd(string condition)
        {
            ConditionsList.Add(new string[] { "and", condition });
        }

        public void AddOr(string condition)
        {
            ConditionsList.Add(new string[] { "or", condition });
        }

        //public string getQuery()
        //{
        //    StringBuilder where = new StringBuilder("");
        //    foreach (string[] itemx in ConditionsList)
        //    {
        //        if (where.ToString() == "")
        //            where.Append(" where ");
        //        else
        //            where.Append(string.Format(" {0} ", itemx[0]));
        //        where.Append(string.Format(" {0} ", itemx[1]));
        //    }
        //    return this.strQuery.Append(where.ToString()).ToString();
        //}

        public string GetQuery()
        {
            string c = GetStrConditions();
            string where = c.Length > 0 ? (" where " + c) : "";
            StrQueryFinal = this.StrQuery.ToString() + where.ToString();
            if ((Suffix??"") != "")
            {
                StrQueryFinal = StrQueryFinal + " " + Suffix;
                Suffix = "";
            }
            return StrQueryFinal;
        }

        public string GetStrConditions()
        {
            StringBuilder strConditions = new StringBuilder("");
            foreach (string[] itemx in ConditionsList)
            {
                if (strConditions.ToString() == "")
                    strConditions.Append("  ");
                else
                    strConditions.Append(string.Format(" {0} ", itemx[0]));
                strConditions.Append(string.Format(" {0} ", itemx[1]));
            }
            return strConditions.ToString();
        }

    }
}