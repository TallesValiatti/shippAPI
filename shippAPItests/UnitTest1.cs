using System;
using Xunit;
using shippAPI;
using shippAPI.Controllers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace shippAPItests
{
    public class UnitTest1
    {
        private ClassStores _objStore;

        public UnitTest1()
        {
            _objStore = new ClassStores();
        }

        //test the distance calculated between to coordinates
        [Fact]
        public void test_distance_Two_Coordinates()
        {
            //shipp Coordinates
            double shippLat = -20.298054;
            double shippLong = -40.300272;

            //My home coordinates
            double homeLat = -20.340318;
            double homeLong = -40.392889;

            //expected distance calculated on google maps (10.74 KM)
            double expectedDistance = 10.74;

            //precision. max diference allowed is 100 m
            int precision = 1;

            double distance = _objStore.DistanceTwoCoordinates(shippLat, shippLong, homeLat, homeLong);

            Assert.Equal(expectedDistance, distance, precision);
        }

        //test the inputValidationMiddleware
        [Theory]
        //test when bad query params were sent
        [InlineData(null, null, 400)]
        [InlineData("a", "b", 400)]
        [InlineData("1.1.1.1","1.2.3.4", 400)]
        //test when valid query params were sent
        [InlineData("-40.0", "-40.0", 200)]
        public async Task test_InputValidationMiddleware(string lati, string longi, int statusCode)
        {
            //Create a new instance of the middleware
            var middleware = new shippAPI.Middlewares.InputValidationMiddleware(
                next: (innerHttpContext) =>
                {
                    return Task.CompletedTask;
                }
            );

            //Create the DefaultHttpContext
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            //set the query params
            if(!string.IsNullOrEmpty(lati))
                context.Request.QueryString = context.Request.QueryString.Add("lat", lati);

            if (!string.IsNullOrEmpty(longi))
                context.Request.QueryString = context.Request.QueryString.Add("long", longi);

            //Call the middleware
            await middleware.Invoke(context);

            //Don't forget to rewind the stream
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = new StreamReader(context.Response.Body).ReadToEnd();

            Assert.Equal(context.Response.StatusCode, statusCode);
        }
    }
}
