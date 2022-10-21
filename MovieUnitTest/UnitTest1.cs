using System;
using Xunit;
using Movie.Controllers;
using Movie.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections;

namespace MovieUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public async Task  TestSearchMovie()
        {
            //arrange
            var term = "Iron Man";
            var totalResult = "101";
            var valuesController = new ValuesController();

            //act
            var movieResult = valuesController.Search(term);
            var okResult = await movieResult as OkObjectResult;
           

            //assert
            Assert.NotNull(movieResult);
            Assert.True(okResult is OkObjectResult);
            Assert.Equal(200, okResult.StatusCode);

        }

        [Fact]
        public async Task TestMovieDetail()
        {
            //arrange
            var title = "Iron Man";
            var valuesController = new ValuesController();

            //act
            var movieResult = valuesController.Detail(title);
            var okResult = await movieResult as OkObjectResult;

            
            //assert
            Assert.NotNull(movieResult);
            Assert.True(okResult is OkObjectResult);
            Assert.IsType<MovieDetail>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

        }

        [Fact]
        public  void TestPreviousMovie()
        {
            //arrange
            var valuesController = new ValuesController();

            //act
            var movieResult = valuesController.PreviousSearch();
            var okResult =  movieResult as OkObjectResult;

            
            //assert
            Assert.NotNull(movieResult);
            Assert.True(okResult is OkObjectResult);
            Assert.Equal(200, okResult.StatusCode);

        }
    }
}
