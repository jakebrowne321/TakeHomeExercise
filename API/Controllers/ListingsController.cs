
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;


namespace API.Controllers
{




    [ApiController]
    [Route("[controller]")]
    public class ListingsController : ControllerBase
    {
        private readonly ILogger<ListingsController> _logger;

        public ListingsController(ILogger<ListingsController> logger)
        {
            _logger = logger;
        }
         
        Dictionary<string, int > _categoryTypes = new Dictionary< string, int>(){
            {"Residential",1  },
            {"Rental",2},
            {"Land",3},
            {"Rural",4},
        };

        Dictionary<string, int> _statusTypes = new Dictionary<string, int>(){
            {"Current",1  },
            {"Withdrawn",2},
            {"Sold",3},
            {"Leased",4},
            {"Off Market",5},
            {"Deleted",6},
        };




        public class ListingDto
        {
            public int ListingId { get; set; }
            public string? StreetNumber { get; set; }
            public string Street { get; set; }
            public string Suburb { get; set; }
            public string State { get; set; }
            public int? Postcode { get; set; }

            public int CategoryType { get; set; }
            public int StatusType { get; set; }
            public string DisplayPrice { get; set; }
            public string Title { get; set; }

    
        }


        public class WrappedDataObject
        {
            public List<ListingDto> items { get; set; }
            public int total { get; set; }

        }

        public ActionResult GetListings(string suburb = null, string categoryType = null, string statusType = null, string skip = null, string take = null)
        {
            string initialQuery = "SELECT *, count(*) OVER() AS full_count FROM Listings";
            string appendedQuery = "";
            //Validation for parameters passed to method
            //Check if expected string values are actually strings not integers
            if (suburb != null)
            {
                bool isNumeric = int.TryParse(suburb, out _);
                if (isNumeric)
                    return UnprocessableEntity("Suburb parameter must be of type string");
                else
                {
                    appendedQuery += " WHERE Suburb = '" + suburb + "'";
                }
            }
            if (categoryType != null)
            {
                bool isNumeric = int.TryParse(categoryType, out _);
                if (isNumeric)
                    return UnprocessableEntity("categoryType parameter must be of type string");
                else
                {

                    //if category name maps to dictionary, append to query
                    if (_categoryTypes.TryGetValue(categoryType, out int categoryId))
                    {
                        if (appendedQuery != "")
                        {
                            appendedQuery += " AND CategoryType = '" + categoryId + "'";
                        }
                        else
                        {
                            appendedQuery += "WHERE CategoryType = '" + categoryId ;
                        }
                    }

                }
            }
            if (statusType != null)
            {
                bool isNumeric = int.TryParse(statusType, out _);
                if (isNumeric)
                    return UnprocessableEntity("statusType parameter must be of type string");
                else
                {


                    //if category name maps to dictionary, append to query
                    if (_statusTypes.TryGetValue(statusType, out int statusId))
                    {
                        if (appendedQuery != "")
                        {
                            appendedQuery += " AND StatusType = '" + statusId + "'";
                        }
                        else
                        {
                            appendedQuery += " WHERE StatusType = '" + statusId + "'";
                        }
                    }
                   
                }

            }

            //Order results by listing id
            appendedQuery += " ORDER BY ListingId";


            //Check that number parameters are of type integer
            if (skip != null)
            {
                bool isNumeric = int.TryParse(skip, out _);
                if (!isNumeric)
                    return UnprocessableEntity("skip parameter must be of type number");
                else
                {
                    appendedQuery += $" OFFSET {skip} ROWS ";
                }


            }
     
            else
            {
                //if skip is not provided, default to 0 so 'take' will work
                appendedQuery += $" OFFSET 0 ROWS";
            }

            
            if (take != null)
            {
                bool isNumeric = int.TryParse(take, out _);
                if (!isNumeric)
                    return UnprocessableEntity("take parameter must be of type number");
                else
                {
                    appendedQuery += $" FETCH NEXT {take} ROWS ONLY";
                }
            }

            //After validation create connection string
            string connString = @"server=JAKE-PC;Integrated Security=SSPI;database=Backend-TakeHomeExercise;Trusted_Connection=True;";

           

            //Join the two connection strings
            var mainQuery = initialQuery + appendedQuery;

            using var con = new SqlConnection(connString);
            con.Open();

            
            using var cmd = new SqlCommand(mainQuery, con);

            using SqlDataReader rdr = cmd.ExecuteReader();

           

            List<ListingDto> listingArrray = new List<ListingDto>();

            var totalRecordCount = 0;

            var counter = 0;

            while (rdr.Read())
            {
                //Only want to assign total once
                if(counter == 0)
                {
                    totalRecordCount = (int)rdr["full_count"];
                    counter++;
                }
                
                var listing = new ListingDto();
                listing.ListingId = (int)rdr["ListingId"];
                listing.StreetNumber = rdr["StreetNumber"] is DBNull ? null : (string)rdr["StreetNumber"];
                listing.Street = rdr["Street"] is DBNull ? null : (string)rdr["Street"];
                listing.Suburb = (string)rdr["Suburb"];
                listing.State = (string)rdr["State"];
                listing.Postcode = (rdr["Postcode"] == null) ? null : (int)rdr["Postcode"];
                listing.CategoryType = (int)rdr["CategoryType"];
                listing.StatusType = (int)rdr["StatusType"];
                listing.DisplayPrice = (string)rdr["DisplayPrice"];
                listing.Title = (string)rdr["Title"] ?? null;
                listingArrray.Add(listing);
            };

            WrappedDataObject returningObj = new WrappedDataObject();
            returningObj.items = listingArrray;
            returningObj.total = totalRecordCount;



            return Ok(returningObj);
        }
    }
}
