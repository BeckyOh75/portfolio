using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests.Therapist;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Sabio.Services
{
    public class TherapistService : ITherapistService
    {
        IDataProvider _data = null;
        ILocationMapper _locationMapper = null;

 
        public TherapistService(IDataProvider data, ILocationMapper locationMapper, IUserProfileMapper profileMapper)
        {
            _data = data;
            _locationMapper = locationMapper;
        }

        public int Add(TherapistAddRequest model, int userId)
        {
            int id = 0;

            string procName = "[dbo].[Therapists_Insert]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {   
                    AddCommonParams(model, col, userId);
                   


                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;

                    col.Add(idOut);

                },
                returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;

                    int.TryParse(oId.ToString(), out id);

                });


            return id;
        }

        public void Update(TherapistUpdateRequest model, int userId)
        {
            string procName = "[dbo].[Therapists_Update]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {   
                    AddCommonParams(model, col, userId);
                    col.AddWithValue("@Id", model.Id);
                   
                },
                returnParameters: null);
        }
        
        public Therapist GetById(int id)
        {
            string procName = "[dbo].[Therapists_SelectById]";

            Therapist therapist = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {

                paramCollection.AddWithValue("@Id", id);

            }, delegate (IDataReader reader, short set)
            {

                therapist = MapTherapist(reader, out int Index);

            });

            return therapist;
        }

        public Paged<Therapist> GetDetails(int pageIndex, int pageSize)
        {
            Paged<Therapist> pagedResult = null;

            List<Therapist> result = null;

            int totalCount = 0;

            _data.ExecuteCmd(
                "dbo.Therapists_SelectAll_Details",
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@PageIndex", pageIndex);
                    parameterCollection.AddWithValue("@PageSize", pageSize);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {

                    Therapist aDetailedTherapist = MapDetailedTherapist(reader, out int index);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index);
                    }

                    if (result == null)
                    {
                        result = new List<Therapist>();
                    }

                    result.Add(aDetailedTherapist);
                }

            );

            if (result != null)
            {
                pagedResult = new Paged<Therapist>(result, pageIndex, pageSize, totalCount);
            }

            return pagedResult;
        }

        public Paged<Therapist> GetSearch(int pageIndex, int pageSize, string query)
        {
            Paged<Therapist> pagedResult = null;

            List<Therapist> result = null;

            int totalCount = 0;

            _data.ExecuteCmd(
                "dbo.Therapists_Search",
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@PageIndex", pageIndex);
                    parameterCollection.AddWithValue("@PageSize", pageSize);
                    parameterCollection.AddWithValue("@Query", query);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {

                    Therapist aTherapist = MapDetailedTherapist(reader, out int index);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index);
                    }


                    if (result == null)
                    {
                        result = new List<Therapist>();
                    }

                    result.Add(aTherapist);
                }

            );
            if (result != null)
            {
                pagedResult = new Paged<Therapist>(result, pageIndex, pageSize, totalCount);
            }

            return pagedResult;
        }

        public Paged<Therapist> GetAll(int pageIndex, int pageSize)
        {
            Paged<Therapist> pagedResult = null;

            List<Therapist> result = null;

            int totalCount = 0;

            _data.ExecuteCmd(
                "dbo.Therapists_SelectAll",
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@PageIndex", pageIndex);
                    parameterCollection.AddWithValue("@PageSize", pageSize);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {

                    Therapist therapist = MapTherapist(reader, out int index);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index);
                    }


                    if (result == null)
                    {
                        result = new List<Therapist>();
                    }

                    result.Add(therapist);
                }

            );
            if (result != null)
            {
                pagedResult = new Paged<Therapist>(result, pageIndex, pageSize, totalCount);
            }

            return pagedResult;
        }

        private static Therapist MapTherapist(IDataReader reader, out int startingIndex)
        {
            Therapist therapist = new Therapist();

            startingIndex = 0;

            therapist.Id = reader.GetSafeInt32(startingIndex++);
            therapist.Email = reader.GetSafeString(startingIndex++);
            therapist.FirstName = reader.GetSafeString(startingIndex++);
            therapist.Mi = reader.GetSafeString(startingIndex++);
            therapist.LastName = reader.GetSafeString(startingIndex++);
            therapist.AvatarUrl = reader.GetSafeString(startingIndex++);
            therapist.Phone = reader.GetSafeString(startingIndex++);
            therapist.LocationId = reader.GetSafeInt32(startingIndex++);
            therapist.IsActive = reader.GetSafeBool(startingIndex++);
            therapist.Notes = reader.GetSafeString(startingIndex++);
            therapist.CreatedBy = reader.GetSafeInt32(startingIndex++);
            therapist.DateCreated = reader.GetDateTime(startingIndex++);
            therapist.DateModified = reader.GetDateTime(startingIndex++);

            return therapist;
        }

        private Therapist MapDetailedTherapist(IDataReader reader, out int startingIndex)
        {
            Therapist aTherapist = new Therapist();

            startingIndex = 0;

            aTherapist.Id = reader.GetSafeInt32(startingIndex++);
            aTherapist.Email = reader.GetSafeString(startingIndex++);
            aTherapist.FirstName = reader.GetSafeString(startingIndex++);
            aTherapist.Mi = reader.GetSafeString(startingIndex++);
            aTherapist.LastName = reader.GetSafeString(startingIndex++);
            aTherapist.AvatarUrl = reader.GetSafeString(startingIndex++);
            aTherapist.Phone = reader.GetSafeString(startingIndex++);

            aTherapist.IsActive = reader.GetSafeBool(startingIndex++);
            aTherapist.Notes = reader.GetSafeString(startingIndex++);
            aTherapist.CreatedBy = reader.GetSafeInt32(startingIndex++);
            aTherapist.DateCreated = reader.GetDateTime(startingIndex++);
            aTherapist.DateModified = reader.GetDateTime(startingIndex++);

            aTherapist.Location = _locationMapper.MapLocation(reader, ref startingIndex);

            return aTherapist;
        }

        private static void AddCommonParams(TherapistAddRequest model, SqlParameterCollection col, int userId)
        {
            col.AddWithValue("@FirstName", model.FirstName);
            col.AddWithValue("@Email", model.Email);
            col.AddWithValue("@Mi", model.Mi);
            col.AddWithValue("@LastName", model.LastName);
            col.AddWithValue("@AvatarUrl", model.AvatarUrl);
            col.AddWithValue("@Phone", model.Phone);
            col.AddWithValue("@LocationId", model.LocationId);
            col.AddWithValue("@IsActive", model.IsActive);
            col.AddWithValue("@Notes", model.Notes);
            col.AddWithValue("@CreatedBy", userId);
        }
    }
}
