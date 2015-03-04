using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UG
{
    public partial class GenerateLease : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGenerateLease_Click(object sender, EventArgs e)
        {
            //Insert Data into database            
            var conStr = ConfigurationManager.ConnectionStrings["DBTriggersConn"].ConnectionString;
            OracleConnection connection = new OracleConnection(conStr);
            OracleCommand cmd = new OracleCommand("STORE_LEASE", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new OracleParameter("v_LeaseID", tbLeaseID.Text));
            cmd.Parameters.Add(new OracleParameter("v_StartDate", tbLeaseStartDate.Text));
            cmd.Parameters.Add(new OracleParameter("v_EndDate", tbLeaseEndDate.Text));
            cmd.Parameters.Add(new OracleParameter("v_NoOfOccupants", tbLeaseOccupants.Text));
            cmd.Parameters.Add(new OracleParameter("v_ManagerID", "E001"));
            cmd.Parameters.Add(new OracleParameter("v_AptNum", tbLeaseAptNum.Text));
            cmd.Parameters.Add(new OracleParameter("v_LDepositAmount", tbLeaseDepositAmt.Text));

            OracleDataAdapter da = new OracleDataAdapter(cmd);
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Lease Generated Successfully!')", true);                              
            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Something went wrong!')", true);
                ClearFields();
                connection.Close();
            }
        }

        protected void btnLeaseCancel_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        public void ClearFields()
        {
            tbLeaseID.Text = string.Empty;
            tbLeaseStartDate.Text = string.Empty;
            tbLeaseEndDate.Text = string.Empty;
            tbLeaseOccupants.Text = string.Empty;
            tbLeaseAptNum.Text = string.Empty;
            tbLeaseDepositAmt.Text = string.Empty;
            lblLeaseAvailability.Text = string.Empty;
        }

        protected void linkCheckLease_Click(object sender, EventArgs e)
        {
            var conStr = ConfigurationManager.ConnectionStrings["DBTriggersConn"].ConnectionString;
            OracleConnection connection = new OracleConnection(conStr);
            OracleCommand cmd = new OracleCommand("LEASE_VALIDATION", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new OracleParameter("v_LeaseID", tbLeaseID.Text));
            cmd.Parameters.Add("Lease_Output", OracleDbType.Int32).Direction = ParameterDirection.Output;

            OracleDataAdapter da = new OracleDataAdapter(cmd);
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                var leaseValid = cmd.Parameters["Lease_Output"].Value;
                if (leaseValid.ToString() == "-99")
                {
                    tbLeaseID.Text = string.Empty;
                    lblLeaseAvailability.Text = "Lease already exists";
                    return;
                }
                else
                {
                    lblLeaseAvailability.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Something went wrong!')", true);
                ClearFields();
                connection.Close();
            }
        }
    }
}