using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class RestoreMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FilesDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    TaskCommentId = table.Column<int>(type: "int", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilesDB", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TasksDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    DueToDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasksDB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TasksDB_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TasksDB_FilesDB_FileId",
                        column: x => x.FileId,
                        principalTable: "FilesDB",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserTaskModel",
                columns: table => new
                {
                    AssignedTasksId = table.Column<int>(type: "int", nullable: false),
                    AssignedUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserTaskModel", x => new { x.AssignedTasksId, x.AssignedUsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserTaskModel_AspNetUsers_AssignedUsersId",
                        column: x => x.AssignedUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserTaskModel_TasksDB_AssignedTasksId",
                        column: x => x.AssignedTasksId,
                        principalTable: "TasksDB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentsDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskModelId = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentsDB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentsDB_FilesDB_FileId",
                        column: x => x.FileId,
                        principalTable: "FilesDB",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommentsDB_TasksDB_TaskModelId",
                        column: x => x.TaskModelId,
                        principalTable: "TasksDB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserTaskModel_AssignedUsersId",
                table: "ApplicationUserTaskModel",
                column: "AssignedUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentsDB_FileId",
                table: "CommentsDB",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentsDB_TaskModelId",
                table: "CommentsDB",
                column: "TaskModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TasksDB_FileId",
                table: "TasksDB",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_TasksDB_OwnerId",
                table: "TasksDB",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserTaskModel");

            migrationBuilder.DropTable(
                name: "CommentsDB");

            migrationBuilder.DropTable(
                name: "TasksDB");

            migrationBuilder.DropTable(
                name: "FilesDB");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");
        }
    }
}
