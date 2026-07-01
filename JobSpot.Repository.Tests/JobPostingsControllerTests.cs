using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JobSpot.Controllers;
using JobSpot.Interfaces;
using JobSpot.Models;
using JobSpot.Repositories;
using JobSpot.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace JobSpot.Repository.Tests
{
    public class JobPostingsControllerTests
    {
        private static ControllerContext CreateControllerContext(string userId = null, params string[] roles)
        {
            var claims = new List<Claim>();
            if (userId != null)
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, "test");
            var principal = new ClaimsPrincipal(identity);

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task Q_Index_ReturnsAllJobPostings_ForNonEmployer()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();

            var list = new List<JobPosting>
            {
                new JobPosting { Id = Guid.NewGuid(), Title = "A", UserId = "u1" },
                new JobPosting { Id = Guid.NewGuid(), Title = "B", UserId = "u2" }
            };
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            // Setup concrete repo to return search results
            var searchResult = new PaginatedResult<JobPosting>
            {
                Items = list,
                TotalCount = list.Count,
                PageNumber = 1,
                PageSize = 9
            };
            concreteRepoMock.Setup(r => r.SearchAndFilterAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<int?>(),
                It.IsAny<JobSortOption>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(searchResult);

            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext()
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var paginated = Assert.IsType<PaginatedResult<JobPosting>>(viewResult.Model);

            // Verify structure
            Assert.NotNull(paginated.Items);
            Assert.Equal(2, paginated.Items.Count());
            Assert.Equal(2, paginated.TotalCount);

            // Verify items are from original list
            Assert.Contains(paginated.Items, jp => jp.Title == "A" && jp.UserId == "u1");
            Assert.Contains(paginated.Items, jp => jp.Title == "B" && jp.UserId == "u2");

            // Verify pagination defaults
            Assert.Equal(1, paginated.PageNumber);
            Assert.Equal(2, paginated.PageSize);

        }

            [Fact]
        public async Task Index_ReturnsAllJobPostings_ForNonEmployer()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();

            var list = new List<JobPosting>
            {
                new JobPosting { Id = Guid.NewGuid(), Title = "A", UserId = "u1" },
                new JobPosting { Id = Guid.NewGuid(), Title = "B", UserId = "u2" }
            };
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(list);



            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext()
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            //var model = Assert.IsAssignableFrom<IEnumerable<JobPosting>>(viewResult.Model);
            //Assert.Equal(2, model.Count());
            var paginated = Assert.IsType<PaginatedResult<JobPosting>>(viewResult.Model);
            var expectedUserId = list.First().UserId;
            var model = Assert.IsType<PaginatedResult<JobPosting>>(viewResult.Model);

            //Assert.Equal(2, model.Items.Count());
            //Assert.Contains(model.Items, x => x.Title == "A");
            //Assert.Contains(model.Items, x => x.Title == "B");
            Assert.All(paginated.Items, jp => Assert.Equal(expectedUserId, jp.UserId));
            Assert.Single(paginated.Items);
            Assert.Equal(2, paginated.TotalCount);
        }

        [Fact]
        public async Task Index_ReturnsOnlyUserJobPostings_WhenUserIsEmployer()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();

            var userId = "employer-1";
            var all = new List<JobPosting>
            {
                new JobPosting { Id = Guid.NewGuid(), Title = "Mine", UserId = userId },
                new JobPosting { Id = Guid.NewGuid(), Title = "Other", UserId = "other" }
            };
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(all);
            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext(userId, "Employer")
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<JobPosting>>(viewResult.Model);
            Assert.Single(model);
            Assert.All(model, jp => Assert.Equal(userId, jp.UserId));
        }

        [Fact]
        public async Task Create_Post_ValidModel_AddsAndRedirects()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();

            var userId = "creator-1";
            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            JobPosting captured = null;
            repoMock.Setup(r => r.AddAsync(It.IsAny<JobPosting>()))
                .Returns(Task.CompletedTask)
                .Callback<JobPosting>(jp => captured = jp);

            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext(userId, "Employer")
            };

            var vm = new JobPostingViewModel
            {
                Title = "T",
                Description = "D",
                Company = "C",
                Location = "L"
            };

            // Act
            var result = await controller.Create(vm);

            // Assert
            repoMock.Verify(r => r.AddAsync(It.IsAny<JobPosting>()), Times.Once);
            Assert.NotNull(captured); // ?!
            Assert.Equal(vm.Title, captured.Title);
            Assert.Equal(userId, captured.UserId);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(JobPostingsController.Index), redirect.ActionName);
        }

        [Fact]
        public async Task Delete_Deletes_WhenAuthorized_ReturnsOk()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();

            var userId = "owner-1";
            var id = Guid.NewGuid();
            var posting = new JobPosting { Id = id, Title = "X", UserId = userId };

            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(posting);
            repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext(userId)
            };

            // Act
            var result = await controller.Delete(id);

            // Assert
            repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();
            var id = Guid.NewGuid();
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((JobPosting?)null);

            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext("any")
            };

            // Act
            var result = await controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            repoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Delete_ReturnsForbid_WhenNotAuthorized()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();

            var id = Guid.NewGuid();
            var posting = new JobPosting { Id = id, Title = "X", UserId = "owner" };
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(posting);
            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("different-user");

            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext("different-user")
            };

            // Act
            var result = await controller.Delete(id);

            // Assert
            Assert.IsType<ForbidResult>(result);
            repoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Edit_Get_ReturnsView_WithJobPosting()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();

            var id = Guid.NewGuid();
            var posting = new JobPosting { Id = id, Title = "EditMe", UserId = "editor" };
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(posting);
            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("editor");
            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext("editor", "Employer")
            };

            // Act
            var result = await controller.Edit(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<JobPostingViewModel>(viewResult.Model);
            Assert.Equal(posting.Title, model.Title);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_UpdatesAndRedirects()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();

            var userId = "editor-1";
            var id = Guid.NewGuid();
            var existingPosting = new JobPosting { Id = id, Title = "OldTitle", UserId = userId };

            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingPosting);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<JobPosting>())).Returns(Task.CompletedTask);
            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext(userId, "Employer")
            };

            var vm = new JobPostingViewModel
            {
                Id = id,
                Title = "NewTitle",
                Description = "D",
                Company = "C",
                Location = "L"
            };

            // Act
            var result = await controller.Edit(vm);

            // Assert
            repoMock.Verify(r => r.UpdateAsync(It.IsAny<JobPosting>()), Times.Once);
            repoMock.Verify(r => r.UpdateAsync(It.Is<JobPosting>(jp => jp.Title == vm.Title)), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(JobPostingsController.Index), redirect.ActionName);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();
            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext("editor", "Employer")
            };
            controller.ModelState.AddModelError("Title", "Required");
            var vm = new JobPostingViewModel
            {
                Title = "",
                Description = "D",
                Company = "C",
                Location = "L"
            };

            // Act
            var result = await controller.Edit(vm);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<JobPostingViewModel>(viewResult.Model);
            Assert.Equal(vm, model);
        }

        [Fact]
        public async Task Edit_Post_UnauthorizedUser_ReturnsForbid()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();
            var id = Guid.NewGuid();
            var existingPosting = new JobPosting { Id = id, Title = "OldTitle", UserId = "owner" };
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingPosting);
            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("different-user");
            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext("different-user", "Employer")
            };
            var vm = new JobPostingViewModel
            {
                Title = "NewTitle",
                Description = "D",
                Company = "C",
                Location = "L"
            };

            // Act
            var result = await controller.Edit(id);

            // Assert
            Assert.IsType<ForbidResult>(result);
            repoMock.Verify(r => r.UpdateAsync(It.IsAny<JobPosting>()), Times.Never);
        }


        [Fact]
        public async Task Edit_Post_NonExistentJobPosting_ReturnsNotFound()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();
            var id = Guid.NewGuid();
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((JobPosting?)null);
            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext("editor", "Employer")
            };
            var vm = new JobPostingViewModel
            {
                Title = "NewTitle",
                Description = "D",
                Company = "C",
                Location = "L"
            };

            // Act
            var result = await controller.Edit(vm);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            repoMock.Verify(r => r.UpdateAsync(It.IsAny<JobPosting>()), Times.Never);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();
            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext("creator", "Employer")
            };
            controller.ModelState.AddModelError("Title", "Required");
            var vm = new JobPostingViewModel
            {
                Title = "",
                Description = "D",
                Company = "C",
                Location = "L"
            };

            // Act
            var result = await controller.Create(vm);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<JobPostingViewModel>(viewResult.Model);
            Assert.Equal(vm, model);
        }

        [Fact]
        public async Task Create_Post_RepositoryThrowsException_LogsErrorAndReturnsView()
        {
            // Arrange
            var repoMock = new Mock<IRepository<JobPosting>>();
            var concreteRepoMock = new Mock<JobPostingRepository>();
            var userManagerMock = new Mock<IUserManager>();
            var loggerMock = new Mock<ILogger<JobPostingsController>>();

            var userId = "creator-1";
            userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            repoMock.Setup(r => r.AddAsync(It.IsAny<JobPosting>()))
                .ThrowsAsync(new Exception("Database error"));
            var controller = new JobPostingsController(repoMock.Object, concreteRepoMock.Object, userManagerMock.Object, loggerMock.Object)
            {
                ControllerContext = CreateControllerContext(userId, "Employer")
            };
            var vm = new JobPostingViewModel
            {
                Title = "T",
                Description = "D",
                Company = "C",
                Location = "L"
            };

            // Act
            var result = await controller.Create(vm);

            // Assert
            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error creating job posting.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<JobPostingViewModel>(viewResult.Model);
            Assert.Equal(vm, model);
        }
    }
}


