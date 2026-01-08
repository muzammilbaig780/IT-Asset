using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAssetTypeIdFromPrinters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the index if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes 
                           WHERE name = 'IX_Printers_AssetTypeId' 
                             AND object_id = OBJECT_ID('Printers'))
                BEGIN
                    DROP INDEX IX_Printers_AssetTypeId ON Printers;
                END
            ");

            // Drop the column if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns 
                           WHERE name = 'AssetTypeId' 
                             AND object_id = OBJECT_ID('Printers'))
                BEGIN
                    ALTER TABLE Printers DROP COLUMN AssetTypeId;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate the column (nullable to avoid FK issues)
            migrationBuilder.AddColumn<int>(
                name: "AssetTypeId",
                table: "Printers",
                type: "int",
                nullable: true);

            // Optionally recreate the index if needed
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes 
                               WHERE name = 'IX_Printers_AssetTypeId' 
                                 AND object_id = OBJECT_ID('Printers'))
                BEGIN
                    CREATE INDEX IX_Printers_AssetTypeId
                    ON Printers(AssetTypeId);
                END
            ");

            // Optionally recreate the foreign key if needed
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
                               WHERE name = 'FK_Printers_AssetTypes_AssetTypeId')
                BEGIN
                    ALTER TABLE Printers
                    ADD CONSTRAINT FK_Printers_AssetTypes_AssetTypeId
                    FOREIGN KEY (AssetTypeId) REFERENCES AssetTypes(AssetTypeId)
                    ON DELETE CASCADE;
                END
            ");
        }
    }
}
