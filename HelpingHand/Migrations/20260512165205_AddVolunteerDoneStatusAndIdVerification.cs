using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpingHand.Migrations
{
    /// <inheritdoc />
    public partial class AddVolunteerDoneStatusAndIdVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdDocumentPath",
                table: "VolunteerApplications",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IdVerified",
                table: "VolunteerApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequesterConfirmedDone",
                table: "HelpRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RequesterFeedback",
                table: "HelpRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "VolunteerConfirmedDone",
                table: "HelpRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdDocumentPath",
                table: "VolunteerApplications");

            migrationBuilder.DropColumn(
                name: "IdVerified",
                table: "VolunteerApplications");

            migrationBuilder.DropColumn(
                name: "RequesterConfirmedDone",
                table: "HelpRequests");

            migrationBuilder.DropColumn(
                name: "RequesterFeedback",
                table: "HelpRequests");

            migrationBuilder.DropColumn(
                name: "VolunteerConfirmedDone",
                table: "HelpRequests");
        }
    }
}
