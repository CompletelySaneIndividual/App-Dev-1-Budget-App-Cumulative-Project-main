﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace EnterpriseBudget.Model
{
    /// <summary>
    /// The different type of jobs that are available
    /// </summary>
    public enum JobTypes
    {
        /// <summary>
        /// No job specified
        /// </summary>
        Nothing = 0,

        /// <summary>
        /// Teacher (faculty)
        /// </summary>
        Faculty = 1,

        /// <summary>
        /// chair person
        /// </summary>
        Chair = 2,

        /// <summary>
        /// job with absolute power
        /// </summary>
        Admin = 3
    } 
    
    /// <summary>
    /// Employee class
    /// </summary>
    public class Employee
    {
        JobTypes _jobType;
        String _jobName;
        int _deptId;
        String _departmentName;
        String _userName;
        
        /// <summary>
        /// what job type does the employee have?
        /// </summary>
        public JobTypes jobType { get { return _jobType; } }

        /// <summary>
        /// what is the department name of the employee
        /// </summary>
        public String departmentName { get { return _departmentName; } }

        /// <summary>
        /// what is the name of the employee's job?
        /// </summary>
        public String jobName { get { return _jobName; } }

        /// <summary>
        /// employee name
        /// </summary>
        public String userName { get { return _userName; } }

        /// <summary>
        /// employee department id
        /// </summary>
        public int deptartmentID { get { return _deptId; } }

        /// <summary>
        /// Verify that there is an employee with username/password
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <returns>an Employee object if such a user exists, null otherwise</returns>
        public static Employee validateUser(String username, String password)
        {
            Employee person = null;
            SqlDataReader rdr = null;
            try
            {
                SqlCommand verifyUser = Connection.cnn.CreateCommand();
                verifyUser.CommandText = "Select name, departmentId, jobId from employees where name = @username and password = @password;";
                verifyUser.Parameters.AddWithValue("@username", username);
                verifyUser.Parameters.AddWithValue("@password", password);

                rdr = verifyUser.ExecuteReader();

                while (rdr.Read())
                {
                    person = new Employee();
                    person._userName = rdr.GetString(0);
                    person._deptId = rdr.GetInt32(1);
                    person._jobType = (JobTypes)rdr.GetInt32(2);
                }

                verifyUser.CommandText = "Select name from deparments where id = @id;";
                verifyUser.Parameters.AddWithValue("@id", person._deptId);

                rdr = verifyUser.ExecuteReader();

                while (rdr.Read())
                {
                    person._departmentName = rdr.GetString(0);
                }

                verifyUser.CommandText = "Select name from jobTitles where id = @id;";
                verifyUser.Parameters.AddWithValue("@id", (int)person._jobType);

                rdr = verifyUser.ExecuteReader();

                while (rdr.Read())
                {
                    person._jobName = rdr.GetString(0);
                }

            }
            catch (Exception e) {
                var txt = e.Message;
            }
            if (rdr != null)
            {
                try { rdr.Close(); } catch { }
            }
            return person;
        }

        private Employee()
        {

        }

    }
}
