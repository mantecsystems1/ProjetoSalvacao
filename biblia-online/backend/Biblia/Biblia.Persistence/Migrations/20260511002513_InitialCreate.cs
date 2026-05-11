using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblia.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bible_versions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bible_versions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Slug = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "chapters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chapters_books_BookId",
                        column: x => x.BookId,
                        principalTable: "books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reading_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BibleVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastReadAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reading_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reading_history_bible_versions_BibleVersionId",
                        column: x => x.BibleVersionId,
                        principalTable: "bible_versions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reading_history_chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reading_history_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "verses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    BibleVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_verses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_verses_bible_versions_BibleVersionId",
                        column: x => x.BibleVersionId,
                        principalTable: "bible_versions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_verses_chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "favorite_verses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    VerseId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favorite_verses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_favorite_verses_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_favorite_verses_verses_VerseId",
                        column: x => x.VerseId,
                        principalTable: "verses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bible_versions_Code",
                table: "bible_versions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_books_Order",
                table: "books",
                column: "Order",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_books_Slug",
                table: "books",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_chapters_BookId_Number",
                table: "chapters",
                columns: new[] { "BookId", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_favorite_verses_UserId_VerseId",
                table: "favorite_verses",
                columns: new[] { "UserId", "VerseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_favorite_verses_VerseId",
                table: "favorite_verses",
                column: "VerseId");

            migrationBuilder.CreateIndex(
                name: "IX_reading_history_BibleVersionId",
                table: "reading_history",
                column: "BibleVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_reading_history_ChapterId",
                table: "reading_history",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_reading_history_UserId_BibleVersionId_ChapterId",
                table: "reading_history",
                columns: new[] { "UserId", "BibleVersionId", "ChapterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_verses_BibleVersionId",
                table: "verses",
                column: "BibleVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_verses_ChapterId_BibleVersionId_Number",
                table: "verses",
                columns: new[] { "ChapterId", "BibleVersionId", "Number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "favorite_verses");

            migrationBuilder.DropTable(
                name: "reading_history");

            migrationBuilder.DropTable(
                name: "verses");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "bible_versions");

            migrationBuilder.DropTable(
                name: "chapters");

            migrationBuilder.DropTable(
                name: "books");
        }
    }
}
