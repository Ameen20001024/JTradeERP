using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JTradeERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Arp01Security : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecurityAuditEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityAuditEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Capability = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.CheckConstraint("CK_RolePermissions_Capability", "[Capability] IN (1, 2, 3, 4)");
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetOtps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OtpHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    FailedAttemptCount = table.Column<int>(type: "int", nullable: false),
                    InvalidatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetOtps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetOtps_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReplacedByTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    RevocationReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code" },
                values: new object[,]
                {
                    { new Guid("028ba3f3-c471-4097-3a8f-75be7ccc0220"), "Billing.Payment" },
                    { new Guid("1d5b4b7a-7398-3efa-e4b6-c53f50f4c8ae"), "Purchasing.GRN" },
                    { new Guid("211b513c-ca05-4be4-7538-6d5e773907d1"), "Admin.Configuration.Manage" },
                    { new Guid("2f56600d-983e-8a80-eeff-9efbc3cea9fb"), "Stock.Transfer" },
                    { new Guid("334b6fd5-e44c-3df7-fe0f-2a50d7a67d36"), "Stock.Post" },
                    { new Guid("3b02943d-eef7-5356-2f51-18f2ddf0448b"), "Billing.Post" },
                    { new Guid("49c79bd3-4a0f-ebcf-7a57-b749479abf3a"), "Billing.SalesOrder" },
                    { new Guid("4a84b29b-f30e-25eb-c6ff-55658157d683"), "Admin.Roles.Manage" },
                    { new Guid("517b99c5-e22c-f7bb-46f1-c96142535401"), "Admin.Users.Manage" },
                    { new Guid("53ffc98c-6cbe-de74-3dee-c20c4d1f51b8"), "Billing.Quotation" },
                    { new Guid("637801ce-0a9f-a5ea-c2e4-08b3be0ed8f8"), "Billing.CreditNote" },
                    { new Guid("7a5411c5-514e-e487-14f4-e995bccf3c71"), "Stock.View" },
                    { new Guid("8f04dfbf-3d95-e298-a91d-c2576b039372"), "MasterData.Manage" },
                    { new Guid("914b8416-2af0-5b46-1fa5-20a6452dbc79"), "Stock.Adjust" },
                    { new Guid("9508e4e2-8407-cb59-5d1f-7259237b3ca8"), "AuditLog.View" },
                    { new Guid("95d09173-c890-cd7f-e4e5-20571f0fd195"), "Customers.Manage" },
                    { new Guid("9feb279d-f8e6-b8b4-7254-e72da6771ceb"), "Billing.Cancel" },
                    { new Guid("a6fba566-769d-2371-b04f-ea63c1c6a48e"), "MasterData.View" },
                    { new Guid("a73741e1-05d5-3dc6-ce9a-ec9562c09c51"), "Suppliers.Manage" },
                    { new Guid("a93135fc-0c14-fd03-0381-a2cf8feb1e73"), "Purchasing.PurchaseOrder" },
                    { new Guid("bfe14f9d-e6db-1da4-5d1f-1fe24aaf33b6"), "Reports.View" },
                    { new Guid("c92a0665-deb3-57fa-8318-662c4946cac8"), "Stock.OpeningBalance" },
                    { new Guid("dea10cd5-1cdf-ad18-de11-f9aabffd4ff3"), "Billing.Invoice" },
                    { new Guid("e3b8f841-2475-8862-ebe3-37e2af667c34"), "Admin.NumberSeries.Manage" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("00261b39-183d-3279-c4e5-2a5d5a6c37bd"), "Read-Only User" },
                    { new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), "Finance Manager" },
                    { new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), "Auditor" },
                    { new Guid("790a9fd1-4890-798f-d696-06819ea39718"), "Warehouse Manager" },
                    { new Guid("87b1443c-4de1-074b-e403-1aabbb09f389"), "Sales User" },
                    { new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), "System Administrator" },
                    { new Guid("98c401b0-8018-260d-cde8-7f9843c65307"), "Warehouse User" },
                    { new Guid("e3ac0f97-860c-8df3-5150-ded859cf99a4"), "Purchase User" },
                    { new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), "Sales Manager" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId", "Capability" },
                values: new object[,]
                {
                    { new Guid("028ba3f3-c471-4097-3a8f-75be7ccc0220"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("1d5b4b7a-7398-3efa-e4b6-c53f50f4c8ae"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("211b513c-ca05-4be4-7538-6d5e773907d1"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("2f56600d-983e-8a80-eeff-9efbc3cea9fb"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 1 },
                    { new Guid("334b6fd5-e44c-3df7-fe0f-2a50d7a67d36"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 1 },
                    { new Guid("3b02943d-eef7-5356-2f51-18f2ddf0448b"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("49c79bd3-4a0f-ebcf-7a57-b749479abf3a"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("53ffc98c-6cbe-de74-3dee-c20c4d1f51b8"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("637801ce-0a9f-a5ea-c2e4-08b3be0ed8f8"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("7a5411c5-514e-e487-14f4-e995bccf3c71"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("8f04dfbf-3d95-e298-a91d-c2576b039372"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("914b8416-2af0-5b46-1fa5-20a6452dbc79"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 1 },
                    { new Guid("9508e4e2-8407-cb59-5d1f-7259237b3ca8"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 1 },
                    { new Guid("95d09173-c890-cd7f-e4e5-20571f0fd195"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("9feb279d-f8e6-b8b4-7254-e72da6771ceb"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("a6fba566-769d-2371-b04f-ea63c1c6a48e"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("a73741e1-05d5-3dc6-ce9a-ec9562c09c51"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("a93135fc-0c14-fd03-0381-a2cf8feb1e73"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("bfe14f9d-e6db-1da4-5d1f-1fe24aaf33b6"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("c92a0665-deb3-57fa-8318-662c4946cac8"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 1 },
                    { new Guid("dea10cd5-1cdf-ad18-de11-f9aabffd4ff3"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("e3b8f841-2475-8862-ebe3-37e2af667c34"), new Guid("026ca40f-46de-8937-b2b7-876d776e3eae"), 4 },
                    { new Guid("028ba3f3-c471-4097-3a8f-75be7ccc0220"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("1d5b4b7a-7398-3efa-e4b6-c53f50f4c8ae"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("211b513c-ca05-4be4-7538-6d5e773907d1"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("2f56600d-983e-8a80-eeff-9efbc3cea9fb"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("334b6fd5-e44c-3df7-fe0f-2a50d7a67d36"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("49c79bd3-4a0f-ebcf-7a57-b749479abf3a"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("53ffc98c-6cbe-de74-3dee-c20c4d1f51b8"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("637801ce-0a9f-a5ea-c2e4-08b3be0ed8f8"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("7a5411c5-514e-e487-14f4-e995bccf3c71"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("914b8416-2af0-5b46-1fa5-20a6452dbc79"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("9508e4e2-8407-cb59-5d1f-7259237b3ca8"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 4 },
                    { new Guid("95d09173-c890-cd7f-e4e5-20571f0fd195"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("9feb279d-f8e6-b8b4-7254-e72da6771ceb"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("a6fba566-769d-2371-b04f-ea63c1c6a48e"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("a73741e1-05d5-3dc6-ce9a-ec9562c09c51"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("a93135fc-0c14-fd03-0381-a2cf8feb1e73"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("bfe14f9d-e6db-1da4-5d1f-1fe24aaf33b6"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 4 },
                    { new Guid("c92a0665-deb3-57fa-8318-662c4946cac8"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("dea10cd5-1cdf-ad18-de11-f9aabffd4ff3"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("e3b8f841-2475-8862-ebe3-37e2af667c34"), new Guid("38363806-7a5d-a160-41ee-7bc4a09460c9"), 1 },
                    { new Guid("1d5b4b7a-7398-3efa-e4b6-c53f50f4c8ae"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 4 },
                    { new Guid("2f56600d-983e-8a80-eeff-9efbc3cea9fb"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 4 },
                    { new Guid("334b6fd5-e44c-3df7-fe0f-2a50d7a67d36"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 4 },
                    { new Guid("49c79bd3-4a0f-ebcf-7a57-b749479abf3a"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 1 },
                    { new Guid("53ffc98c-6cbe-de74-3dee-c20c4d1f51b8"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 1 },
                    { new Guid("637801ce-0a9f-a5ea-c2e4-08b3be0ed8f8"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 1 },
                    { new Guid("7a5411c5-514e-e487-14f4-e995bccf3c71"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 4 },
                    { new Guid("8f04dfbf-3d95-e298-a91d-c2576b039372"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 4 },
                    { new Guid("914b8416-2af0-5b46-1fa5-20a6452dbc79"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 4 },
                    { new Guid("9508e4e2-8407-cb59-5d1f-7259237b3ca8"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 1 },
                    { new Guid("95d09173-c890-cd7f-e4e5-20571f0fd195"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 1 },
                    { new Guid("a6fba566-769d-2371-b04f-ea63c1c6a48e"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 4 },
                    { new Guid("a73741e1-05d5-3dc6-ce9a-ec9562c09c51"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 1 },
                    { new Guid("a93135fc-0c14-fd03-0381-a2cf8feb1e73"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 1 },
                    { new Guid("bfe14f9d-e6db-1da4-5d1f-1fe24aaf33b6"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 4 },
                    { new Guid("c92a0665-deb3-57fa-8318-662c4946cac8"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 4 },
                    { new Guid("dea10cd5-1cdf-ad18-de11-f9aabffd4ff3"), new Guid("790a9fd1-4890-798f-d696-06819ea39718"), 1 },
                    { new Guid("028ba3f3-c471-4097-3a8f-75be7ccc0220"), new Guid("87b1443c-4de1-074b-e403-1aabbb09f389"), 1 },
                    { new Guid("49c79bd3-4a0f-ebcf-7a57-b749479abf3a"), new Guid("87b1443c-4de1-074b-e403-1aabbb09f389"), 2 },
                    { new Guid("53ffc98c-6cbe-de74-3dee-c20c4d1f51b8"), new Guid("87b1443c-4de1-074b-e403-1aabbb09f389"), 2 },
                    { new Guid("637801ce-0a9f-a5ea-c2e4-08b3be0ed8f8"), new Guid("87b1443c-4de1-074b-e403-1aabbb09f389"), 2 },
                    { new Guid("7a5411c5-514e-e487-14f4-e995bccf3c71"), new Guid("87b1443c-4de1-074b-e403-1aabbb09f389"), 1 },
                    { new Guid("95d09173-c890-cd7f-e4e5-20571f0fd195"), new Guid("87b1443c-4de1-074b-e403-1aabbb09f389"), 2 },
                    { new Guid("a6fba566-769d-2371-b04f-ea63c1c6a48e"), new Guid("87b1443c-4de1-074b-e403-1aabbb09f389"), 1 },
                    { new Guid("bfe14f9d-e6db-1da4-5d1f-1fe24aaf33b6"), new Guid("87b1443c-4de1-074b-e403-1aabbb09f389"), 1 },
                    { new Guid("dea10cd5-1cdf-ad18-de11-f9aabffd4ff3"), new Guid("87b1443c-4de1-074b-e403-1aabbb09f389"), 2 },
                    { new Guid("028ba3f3-c471-4097-3a8f-75be7ccc0220"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("1d5b4b7a-7398-3efa-e4b6-c53f50f4c8ae"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("211b513c-ca05-4be4-7538-6d5e773907d1"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("2f56600d-983e-8a80-eeff-9efbc3cea9fb"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("334b6fd5-e44c-3df7-fe0f-2a50d7a67d36"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("3b02943d-eef7-5356-2f51-18f2ddf0448b"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("49c79bd3-4a0f-ebcf-7a57-b749479abf3a"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("4a84b29b-f30e-25eb-c6ff-55658157d683"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("517b99c5-e22c-f7bb-46f1-c96142535401"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("53ffc98c-6cbe-de74-3dee-c20c4d1f51b8"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("637801ce-0a9f-a5ea-c2e4-08b3be0ed8f8"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("7a5411c5-514e-e487-14f4-e995bccf3c71"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("8f04dfbf-3d95-e298-a91d-c2576b039372"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("914b8416-2af0-5b46-1fa5-20a6452dbc79"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("9508e4e2-8407-cb59-5d1f-7259237b3ca8"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("95d09173-c890-cd7f-e4e5-20571f0fd195"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("9feb279d-f8e6-b8b4-7254-e72da6771ceb"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("a6fba566-769d-2371-b04f-ea63c1c6a48e"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("a73741e1-05d5-3dc6-ce9a-ec9562c09c51"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("a93135fc-0c14-fd03-0381-a2cf8feb1e73"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("bfe14f9d-e6db-1da4-5d1f-1fe24aaf33b6"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("c92a0665-deb3-57fa-8318-662c4946cac8"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("dea10cd5-1cdf-ad18-de11-f9aabffd4ff3"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("e3b8f841-2475-8862-ebe3-37e2af667c34"), new Guid("8de5e7b1-97cb-04af-20b1-61861c78ba72"), 4 },
                    { new Guid("1d5b4b7a-7398-3efa-e4b6-c53f50f4c8ae"), new Guid("98c401b0-8018-260d-cde8-7f9843c65307"), 2 },
                    { new Guid("2f56600d-983e-8a80-eeff-9efbc3cea9fb"), new Guid("98c401b0-8018-260d-cde8-7f9843c65307"), 2 },
                    { new Guid("334b6fd5-e44c-3df7-fe0f-2a50d7a67d36"), new Guid("98c401b0-8018-260d-cde8-7f9843c65307"), 3 },
                    { new Guid("7a5411c5-514e-e487-14f4-e995bccf3c71"), new Guid("98c401b0-8018-260d-cde8-7f9843c65307"), 4 },
                    { new Guid("914b8416-2af0-5b46-1fa5-20a6452dbc79"), new Guid("98c401b0-8018-260d-cde8-7f9843c65307"), 2 },
                    { new Guid("a6fba566-769d-2371-b04f-ea63c1c6a48e"), new Guid("98c401b0-8018-260d-cde8-7f9843c65307"), 1 },
                    { new Guid("bfe14f9d-e6db-1da4-5d1f-1fe24aaf33b6"), new Guid("98c401b0-8018-260d-cde8-7f9843c65307"), 1 },
                    { new Guid("1d5b4b7a-7398-3efa-e4b6-c53f50f4c8ae"), new Guid("e3ac0f97-860c-8df3-5150-ded859cf99a4"), 1 },
                    { new Guid("7a5411c5-514e-e487-14f4-e995bccf3c71"), new Guid("e3ac0f97-860c-8df3-5150-ded859cf99a4"), 1 },
                    { new Guid("8f04dfbf-3d95-e298-a91d-c2576b039372"), new Guid("e3ac0f97-860c-8df3-5150-ded859cf99a4"), 4 },
                    { new Guid("95d09173-c890-cd7f-e4e5-20571f0fd195"), new Guid("e3ac0f97-860c-8df3-5150-ded859cf99a4"), 1 },
                    { new Guid("a6fba566-769d-2371-b04f-ea63c1c6a48e"), new Guid("e3ac0f97-860c-8df3-5150-ded859cf99a4"), 4 },
                    { new Guid("a73741e1-05d5-3dc6-ce9a-ec9562c09c51"), new Guid("e3ac0f97-860c-8df3-5150-ded859cf99a4"), 2 },
                    { new Guid("a93135fc-0c14-fd03-0381-a2cf8feb1e73"), new Guid("e3ac0f97-860c-8df3-5150-ded859cf99a4"), 4 },
                    { new Guid("bfe14f9d-e6db-1da4-5d1f-1fe24aaf33b6"), new Guid("e3ac0f97-860c-8df3-5150-ded859cf99a4"), 1 },
                    { new Guid("028ba3f3-c471-4097-3a8f-75be7ccc0220"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 1 },
                    { new Guid("1d5b4b7a-7398-3efa-e4b6-c53f50f4c8ae"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 1 },
                    { new Guid("3b02943d-eef7-5356-2f51-18f2ddf0448b"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 3 },
                    { new Guid("49c79bd3-4a0f-ebcf-7a57-b749479abf3a"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 4 },
                    { new Guid("53ffc98c-6cbe-de74-3dee-c20c4d1f51b8"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 4 },
                    { new Guid("637801ce-0a9f-a5ea-c2e4-08b3be0ed8f8"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 3 },
                    { new Guid("7a5411c5-514e-e487-14f4-e995bccf3c71"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 1 },
                    { new Guid("8f04dfbf-3d95-e298-a91d-c2576b039372"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 4 },
                    { new Guid("9508e4e2-8407-cb59-5d1f-7259237b3ca8"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 1 },
                    { new Guid("95d09173-c890-cd7f-e4e5-20571f0fd195"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 4 },
                    { new Guid("9feb279d-f8e6-b8b4-7254-e72da6771ceb"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 3 },
                    { new Guid("a6fba566-769d-2371-b04f-ea63c1c6a48e"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 4 },
                    { new Guid("a93135fc-0c14-fd03-0381-a2cf8feb1e73"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 1 },
                    { new Guid("bfe14f9d-e6db-1da4-5d1f-1fe24aaf33b6"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 4 },
                    { new Guid("dea10cd5-1cdf-ad18-de11-f9aabffd4ff3"), new Guid("f4def579-6e3d-0c71-35ee-56fc1cf5f9c2"), 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetOtps_UserId_ExpiresAt",
                table: "PasswordResetOtps",
                columns: new[] { "UserId", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_FamilyId_RevokedAt",
                table: "RefreshTokens",
                columns: new[] { "FamilyId", "RevokedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_RevokedAt",
                table: "RefreshTokens",
                columns: new[] { "UserId", "RevokedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecurityAuditEntries_OccurredAt",
                table: "SecurityAuditEntries",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordResetOtps");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SecurityAuditEntries");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
