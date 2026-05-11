using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliaOnline.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CanonicalNumber = table.Column<int>(type: "integer", nullable: false),
                    Slug = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "languages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    NativeName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EnglishName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsRtl = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_languages", x => x.Id);
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
                name: "bible_versions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bible_versions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bible_versions_languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "book_titles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BibleVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_book_titles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_book_titles_bible_versions_BibleVersionId",
                        column: x => x.BibleVersionId,
                        principalTable: "bible_versions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_book_titles_books_BookId",
                        column: x => x.BookId,
                        principalTable: "books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "favorite_verses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BibleVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChapterNumber = table.Column<int>(type: "integer", nullable: false),
                    VerseNumber = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favorite_verses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_favorite_verses_bible_versions_BibleVersionId",
                        column: x => x.BibleVersionId,
                        principalTable: "bible_versions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_favorite_verses_books_BookId",
                        column: x => x.BookId,
                        principalTable: "books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_favorite_verses_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
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
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChapterNumber = table.Column<int>(type: "integer", nullable: false),
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
                        name: "FK_reading_history_books_BookId",
                        column: x => x.BookId,
                        principalTable: "books",
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
                    BibleVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChapterNumber = table.Column<int>(type: "integer", nullable: false),
                    VerseNumber = table.Column<int>(type: "integer", nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_verses_books_BookId",
                        column: x => x.BookId,
                        principalTable: "books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bible_versions_LanguageId_Code",
                table: "bible_versions",
                columns: new[] { "LanguageId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_book_titles_BibleVersionId_BookId",
                table: "book_titles",
                columns: new[] { "BibleVersionId", "BookId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_book_titles_BookId",
                table: "book_titles",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_books_CanonicalNumber",
                table: "books",
                column: "CanonicalNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_books_Slug",
                table: "books",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_favorite_verses_BibleVersionId",
                table: "favorite_verses",
                column: "BibleVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_favorite_verses_BookId",
                table: "favorite_verses",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_favorite_verses_UserId_BibleVersionId_BookId_ChapterNumber_~",
                table: "favorite_verses",
                columns: new[] { "UserId", "BibleVersionId", "BookId", "ChapterNumber", "VerseNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_languages_Code",
                table: "languages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reading_history_BibleVersionId",
                table: "reading_history",
                column: "BibleVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_reading_history_BookId",
                table: "reading_history",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_reading_history_UserId_BibleVersionId_BookId_ChapterNumber",
                table: "reading_history",
                columns: new[] { "UserId", "BibleVersionId", "BookId", "ChapterNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_verses_BibleVersionId_BookId_ChapterNumber_VerseNumber",
                table: "verses",
                columns: new[] { "BibleVersionId", "BookId", "ChapterNumber", "VerseNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_verses_BookId",
                table: "verses",
                column: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "book_titles");

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
                name: "books");

            migrationBuilder.DropTable(
                name: "languages");
        }
    }
}
