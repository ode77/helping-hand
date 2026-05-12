using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpingHand.Migrations
{
    /// <inheritdoc />
    public partial class AddAllNewFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "HelpRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Urgency",
                table: "HelpRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompletedHelpCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SkillCategories",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RelatedRequestId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestComments",
                columns: table => new
                {
                    RequestCommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HelpRequestId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestComments", x => x.RequestCommentId);
                    table.ForeignKey(
                        name: "FK_RequestComments_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestComments_HelpRequests_HelpRequestId",
                        column: x => x.HelpRequestId,
                        principalTable: "HelpRequests",
                        principalColumn: "HelpRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestTemplates",
                columns: table => new
                {
                    RequestTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Urgency = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTemplates", x => x.RequestTemplateId);
                    table.ForeignKey(
                        name: "FK_RequestTemplates_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestTemplates_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VolunteerApplications",
                columns: table => new
                {
                    VolunteerApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HelpRequestId = table.Column<int>(type: "int", nullable: false),
                    VolunteerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerApplications", x => x.VolunteerApplicationId);
                    table.ForeignKey(
                        name: "FK_VolunteerApplications_AspNetUsers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VolunteerApplications_HelpRequests_HelpRequestId",
                        column: x => x.HelpRequestId,
                        principalTable: "HelpRequests",
                        principalColumn: "HelpRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VolunteerRatings",
                columns: table => new
                {
                    VolunteerRatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HelpRequestId = table.Column<int>(type: "int", nullable: false),
                    VolunteerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequesterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stars = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    RatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerRatings", x => x.VolunteerRatingId);
                    table.ForeignKey(
                        name: "FK_VolunteerRatings_AspNetUsers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VolunteerRatings_HelpRequests_HelpRequestId",
                        column: x => x.HelpRequestId,
                        principalTable: "HelpRequests",
                        principalColumn: "HelpRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HelpRequests_Urgency",
                table: "HelpRequests",
                column: "Urgency");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestComments_AuthorId",
                table: "RequestComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestComments_HelpRequestId",
                table: "RequestComments",
                column: "HelpRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTemplates_CategoryId",
                table: "RequestTemplates",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTemplates_OwnerId",
                table: "RequestTemplates",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerApplications_HelpRequestId",
                table: "VolunteerApplications",
                column: "HelpRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerApplications_VolunteerId",
                table: "VolunteerApplications",
                column: "VolunteerId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerRatings_HelpRequestId",
                table: "VolunteerRatings",
                column: "HelpRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerRatings_VolunteerId",
                table: "VolunteerRatings",
                column: "VolunteerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RequestComments");

            migrationBuilder.DropTable(
                name: "RequestTemplates");

            migrationBuilder.DropTable(
                name: "VolunteerApplications");

            migrationBuilder.DropTable(
                name: "VolunteerRatings");

            migrationBuilder.DropIndex(
                name: "IX_HelpRequests_Urgency",
                table: "HelpRequests");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "HelpRequests");

            migrationBuilder.DropColumn(
                name: "Urgency",
                table: "HelpRequests");

            migrationBuilder.DropColumn(
                name: "CompletedHelpCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SkillCategories",
                table: "AspNetUsers");
        }
    }
}
