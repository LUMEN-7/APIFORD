using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIFORD.Migrations
{
    /// <inheritdoc />
    public partial class Traduzindo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AdditionalSources");

            migrationBuilder.DropTable(
                name: "CarroModes");

            migrationBuilder.DropTable(
                name: "ConsumeSources");

            migrationBuilder.DropTable(
                name: "DimensionSources");

            migrationBuilder.DropTable(
                name: "ModeSources");

            migrationBuilder.DropTable(
                name: "SavedModels");

            migrationBuilder.DropTable(
                name: "SpecificationSources");

            migrationBuilder.DropTable(
                name: "TireSources");

            migrationBuilder.DropTable(
                name: "Additionals");

            migrationBuilder.DropTable(
                name: "Consumes");

            migrationBuilder.DropTable(
                name: "Dimensions");

            migrationBuilder.DropTable(
                name: "Modes");

            migrationBuilder.DropTable(
                name: "Specifications");

            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.DropTable(
                name: "Tires");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Carros",
                table: "Carros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "notifications");

            migrationBuilder.RenameTable(
                name: "Carros",
                newName: "carros");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "notifications",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Subtitle",
                table: "notifications",
                newName: "subtitle");

            migrationBuilder.RenameColumn(
                name: "Mensagem",
                table: "notifications",
                newName: "mensagem");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "notifications",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "notifications",
                newName: "titulo");

            migrationBuilder.RenameColumn(
                name: "Read",
                table: "notifications",
                newName: "lida");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "notifications",
                newName: "excluido");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "notifications",
                newName: "data_criacao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "carros",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "carros",
                newName: "ano");

            migrationBuilder.RenameColumn(
                name: "Model",
                table: "carros",
                newName: "modelo");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "carros",
                newName: "excluido");

            migrationBuilder.RenameColumn(
                name: "Brand",
                table: "carros",
                newName: "marca");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "AspNetUserTokens",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AspNetUserTokens",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                newName: "login_provider");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AspNetUserTokens",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "AspNetUsers",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AspNetUsers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "AspNetUsers",
                newName: "user_name");

            migrationBuilder.RenameColumn(
                name: "TwoFactorEnabled",
                table: "AspNetUsers",
                newName: "two_factor_enabled");

            migrationBuilder.RenameColumn(
                name: "SecurityStamp",
                table: "AspNetUsers",
                newName: "security_stamp");

            migrationBuilder.RenameColumn(
                name: "PhoneNumberConfirmed",
                table: "AspNetUsers",
                newName: "phone_number_confirmed");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "AspNetUsers",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "AspNetUsers",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "NormalizedUserName",
                table: "AspNetUsers",
                newName: "normalized_user_name");

            migrationBuilder.RenameColumn(
                name: "NormalizedEmail",
                table: "AspNetUsers",
                newName: "normalized_email");

            migrationBuilder.RenameColumn(
                name: "LockoutEnd",
                table: "AspNetUsers",
                newName: "lockout_end");

            migrationBuilder.RenameColumn(
                name: "LockoutEnabled",
                table: "AspNetUsers",
                newName: "lockout_enabled");

            migrationBuilder.RenameColumn(
                name: "EmailConfirmed",
                table: "AspNetUsers",
                newName: "email_confirmed");

            migrationBuilder.RenameColumn(
                name: "ConcurrencyStamp",
                table: "AspNetUsers",
                newName: "concurrency_stamp");

            migrationBuilder.RenameColumn(
                name: "AccessFailedCount",
                table: "AspNetUsers",
                newName: "access_failed_count");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "AspNetUsers",
                newName: "excluido");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "AspNetUserRoles",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AspNetUserRoles",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                newName: "ix_asp_net_user_roles_role_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AspNetUserLogins",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "ProviderDisplayName",
                table: "AspNetUserLogins",
                newName: "provider_display_name");

            migrationBuilder.RenameColumn(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                newName: "provider_key");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                newName: "login_provider");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                newName: "ix_asp_net_user_logins_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AspNetUserClaims",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AspNetUserClaims",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "ClaimValue",
                table: "AspNetUserClaims",
                newName: "claim_value");

            migrationBuilder.RenameColumn(
                name: "ClaimType",
                table: "AspNetUserClaims",
                newName: "claim_type");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                newName: "ix_asp_net_user_claims_user_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AspNetRoles",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AspNetRoles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "NormalizedName",
                table: "AspNetRoles",
                newName: "normalized_name");

            migrationBuilder.RenameColumn(
                name: "ConcurrencyStamp",
                table: "AspNetRoles",
                newName: "concurrency_stamp");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AspNetRoleClaims",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "AspNetRoleClaims",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "ClaimValue",
                table: "AspNetRoleClaims",
                newName: "claim_value");

            migrationBuilder.RenameColumn(
                name: "ClaimType",
                table: "AspNetRoleClaims",
                newName: "claim_type");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                newName: "ix_asp_net_role_claims_role_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_notifications",
                table: "notifications",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_carros",
                table: "carros",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_user_tokens",
                table: "AspNetUserTokens",
                columns: new[] { "user_id", "login_provider", "name" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_users",
                table: "AspNetUsers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_user_roles",
                table: "AspNetUserRoles",
                columns: new[] { "user_id", "role_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_user_logins",
                table: "AspNetUserLogins",
                columns: new[] { "login_provider", "provider_key" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_user_claims",
                table: "AspNetUserClaims",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_roles",
                table: "AspNetRoles",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_role_claims",
                table: "AspNetRoleClaims",
                column: "id");

            migrationBuilder.CreateTable(
                name: "consumos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    carro_id = table.Column<int>(type: "int", nullable: false),
                    cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    estrada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    excluido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_consumos", x => x.id);
                    table.ForeignKey(
                        name: "fk_consumos_carros_carro_id",
                        column: x => x.carro_id,
                        principalTable: "carros",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dimensoes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    carro_id = table.Column<int>(type: "int", nullable: false),
                    comprimento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    largura = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    altura = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    entre_eixos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    excluido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dimensoes", x => x.id);
                    table.ForeignKey(
                        name: "fk_dimensoes_carros_carro_id",
                        column: x => x.carro_id,
                        principalTable: "carros",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "especificacoes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    carro_id = table.Column<int>(type: "int", nullable: false),
                    potencia = table.Column<int>(type: "int", nullable: false),
                    torque = table.Column<int>(type: "int", nullable: false),
                    rpm_potencia = table.Column<int>(type: "int", nullable: false),
                    rpm_torque = table.Column<int>(type: "int", nullable: false),
                    transmissao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tracao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    excluido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_especificacoes", x => x.id);
                    table.ForeignKey(
                        name: "fk_especificacoes_carros_carro_id",
                        column: x => x.carro_id,
                        principalTable: "carros",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "extras",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    carro_id = table.Column<int>(type: "int", nullable: false),
                    capacidade_tanque = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tipo_combustivel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    capacidade_carga = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    capacidade_reboque = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    excluido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_extras", x => x.id);
                    table.ForeignKey(
                        name: "fk_extras_carros_carro_id",
                        column: x => x.carro_id,
                        principalTable: "carros",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "fontes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    confiabilidade = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    data_adicao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    excluido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fontes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "modelo_salvos",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    carro_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_modelo_salvos", x => new { x.user_id, x.carro_id });
                    table.ForeignKey(
                        name: "fk_modelo_salvos_carros_carro_id",
                        column: x => x.carro_id,
                        principalTable: "carros",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_modelo_salvos_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "modos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_modos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pneus",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    carro_id = table.Column<int>(type: "int", nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    aro = table.Column<int>(type: "int", nullable: false),
                    largura = table.Column<int>(type: "int", nullable: false),
                    perfil = table.Column<int>(type: "int", nullable: false),
                    excluido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pneus", x => x.id);
                    table.ForeignKey(
                        name: "fk_pneus_carros_carro_id",
                        column: x => x.carro_id,
                        principalTable: "carros",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "consumo_fontes",
                columns: table => new
                {
                    consumo_id = table.Column<int>(type: "int", nullable: false),
                    fonte_id = table.Column<int>(type: "int", nullable: false),
                    data_coleta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_referencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_consumo_fontes", x => new { x.consumo_id, x.fonte_id });
                    table.ForeignKey(
                        name: "fk_consumo_fontes_consumos_consumo_id",
                        column: x => x.consumo_id,
                        principalTable: "consumos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_consumo_fontes_fontes_fonte_id",
                        column: x => x.fonte_id,
                        principalTable: "fontes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dimensao_fontes",
                columns: table => new
                {
                    dimensao_id = table.Column<int>(type: "int", nullable: false),
                    fonte_id = table.Column<int>(type: "int", nullable: false),
                    data_coleta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_referencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dimensao_fontes", x => new { x.dimensao_id, x.fonte_id });
                    table.ForeignKey(
                        name: "fk_dimensao_fontes_dimensoes_dimensao_id",
                        column: x => x.dimensao_id,
                        principalTable: "dimensoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dimensao_fontes_fontes_fonte_id",
                        column: x => x.fonte_id,
                        principalTable: "fontes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "especificacao_fontes",
                columns: table => new
                {
                    especificacao_id = table.Column<int>(type: "int", nullable: false),
                    fonte_id = table.Column<int>(type: "int", nullable: false),
                    data_coleta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_referencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_especificacao_fontes", x => new { x.especificacao_id, x.fonte_id });
                    table.ForeignKey(
                        name: "fk_especificacao_fontes_especificacoes_especificacao_id",
                        column: x => x.especificacao_id,
                        principalTable: "especificacoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_especificacao_fontes_fontes_fonte_id",
                        column: x => x.fonte_id,
                        principalTable: "fontes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "extra_fontes",
                columns: table => new
                {
                    extra_id = table.Column<int>(type: "int", nullable: false),
                    fonte_id = table.Column<int>(type: "int", nullable: false),
                    data_coleta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_referencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_extra_fontes", x => new { x.extra_id, x.fonte_id });
                    table.ForeignKey(
                        name: "fk_extra_fontes_extras_extra_id",
                        column: x => x.extra_id,
                        principalTable: "extras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_extra_fontes_fontes_fonte_id",
                        column: x => x.fonte_id,
                        principalTable: "fontes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "carro_modos",
                columns: table => new
                {
                    carro_id = table.Column<int>(type: "int", nullable: false),
                    modo_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_carro_modos", x => new { x.carro_id, x.modo_id });
                    table.ForeignKey(
                        name: "fk_carro_modos_carros_carro_id",
                        column: x => x.carro_id,
                        principalTable: "carros",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_carro_modos_modos_modo_id",
                        column: x => x.modo_id,
                        principalTable: "modos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "modo_fontes",
                columns: table => new
                {
                    modo_id = table.Column<int>(type: "int", nullable: false),
                    fonte_id = table.Column<int>(type: "int", nullable: false),
                    data_coleta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_referencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modo_fonte_fonte_id = table.Column<int>(type: "int", nullable: true),
                    modo_fonte_modo_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_modo_fontes", x => new { x.modo_id, x.fonte_id });
                    table.ForeignKey(
                        name: "fk_modo_fontes_fontes_fonte_id",
                        column: x => x.fonte_id,
                        principalTable: "fontes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_modo_fontes_modo_fontes_modo_fonte_modo_id_modo_fonte_fonte_id",
                        columns: x => new { x.modo_fonte_modo_id, x.modo_fonte_fonte_id },
                        principalTable: "modo_fontes",
                        principalColumns: new[] { "modo_id", "fonte_id" });
                    table.ForeignKey(
                        name: "fk_modo_fontes_modos_modo_id",
                        column: x => x.modo_id,
                        principalTable: "modos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pneu_fontes",
                columns: table => new
                {
                    pneu_id = table.Column<int>(type: "int", nullable: false),
                    fonte_id = table.Column<int>(type: "int", nullable: false),
                    data_coleta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_referencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pneu_fontes", x => new { x.pneu_id, x.fonte_id });
                    table.ForeignKey(
                        name: "fk_pneu_fontes_fontes_fonte_id",
                        column: x => x.fonte_id,
                        principalTable: "fontes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pneu_fontes_pneus_pneu_id",
                        column: x => x.pneu_id,
                        principalTable: "pneus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "normalized_user_name",
                unique: true,
                filter: "[normalized_user_name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "normalized_name",
                unique: true,
                filter: "[normalized_name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_carro_modos_modo_id",
                table: "carro_modos",
                column: "modo_id");

            migrationBuilder.CreateIndex(
                name: "ix_consumo_fontes_fonte_id",
                table: "consumo_fontes",
                column: "fonte_id");

            migrationBuilder.CreateIndex(
                name: "ix_consumos_carro_id",
                table: "consumos",
                column: "carro_id");

            migrationBuilder.CreateIndex(
                name: "ix_dimensao_fontes_fonte_id",
                table: "dimensao_fontes",
                column: "fonte_id");

            migrationBuilder.CreateIndex(
                name: "ix_dimensoes_carro_id",
                table: "dimensoes",
                column: "carro_id");

            migrationBuilder.CreateIndex(
                name: "ix_especificacao_fontes_fonte_id",
                table: "especificacao_fontes",
                column: "fonte_id");

            migrationBuilder.CreateIndex(
                name: "ix_especificacoes_carro_id",
                table: "especificacoes",
                column: "carro_id");

            migrationBuilder.CreateIndex(
                name: "ix_extra_fontes_fonte_id",
                table: "extra_fontes",
                column: "fonte_id");

            migrationBuilder.CreateIndex(
                name: "ix_extras_carro_id",
                table: "extras",
                column: "carro_id");

            migrationBuilder.CreateIndex(
                name: "ix_modelo_salvos_carro_id",
                table: "modelo_salvos",
                column: "carro_id");

            migrationBuilder.CreateIndex(
                name: "ix_modo_fontes_fonte_id",
                table: "modo_fontes",
                column: "fonte_id");

            migrationBuilder.CreateIndex(
                name: "ix_modo_fontes_modo_fonte_modo_id_modo_fonte_fonte_id",
                table: "modo_fontes",
                columns: new[] { "modo_fonte_modo_id", "modo_fonte_fonte_id" });

            migrationBuilder.CreateIndex(
                name: "ix_pneu_fontes_fonte_id",
                table: "pneu_fontes",
                column: "fonte_id");

            migrationBuilder.CreateIndex(
                name: "ix_pneus_carro_id",
                table: "pneus",
                column: "carro_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                table: "AspNetRoleClaims",
                column: "role_id",
                principalTable: "AspNetRoles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_claims_asp_net_users_user_id",
                table: "AspNetUserClaims",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_logins_asp_net_users_user_id",
                table: "AspNetUserLogins",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                table: "AspNetUserRoles",
                column: "role_id",
                principalTable: "AspNetRoles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_roles_asp_net_users_user_id",
                table: "AspNetUserRoles",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                table: "AspNetUserTokens",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_claims_asp_net_users_user_id",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_logins_asp_net_users_user_id",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_roles_asp_net_users_user_id",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                table: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "carro_modos");

            migrationBuilder.DropTable(
                name: "consumo_fontes");

            migrationBuilder.DropTable(
                name: "dimensao_fontes");

            migrationBuilder.DropTable(
                name: "especificacao_fontes");

            migrationBuilder.DropTable(
                name: "extra_fontes");

            migrationBuilder.DropTable(
                name: "modelo_salvos");

            migrationBuilder.DropTable(
                name: "modo_fontes");

            migrationBuilder.DropTable(
                name: "pneu_fontes");

            migrationBuilder.DropTable(
                name: "consumos");

            migrationBuilder.DropTable(
                name: "dimensoes");

            migrationBuilder.DropTable(
                name: "especificacoes");

            migrationBuilder.DropTable(
                name: "extras");

            migrationBuilder.DropTable(
                name: "modos");

            migrationBuilder.DropTable(
                name: "fontes");

            migrationBuilder.DropTable(
                name: "pneus");

            migrationBuilder.DropPrimaryKey(
                name: "pk_notifications",
                table: "notifications");

            migrationBuilder.DropPrimaryKey(
                name: "pk_carros",
                table: "carros");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_user_tokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_users",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_user_roles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_user_logins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_user_claims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_roles",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_role_claims",
                table: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "notifications",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "carros",
                newName: "Carros");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Notifications",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "subtitle",
                table: "Notifications",
                newName: "Subtitle");

            migrationBuilder.RenameColumn(
                name: "mensagem",
                table: "Notifications",
                newName: "Mensagem");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Notifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "titulo",
                table: "Notifications",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "lida",
                table: "Notifications",
                newName: "Read");

            migrationBuilder.RenameColumn(
                name: "excluido",
                table: "Notifications",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "data_criacao",
                table: "Notifications",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Carros",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "modelo",
                table: "Carros",
                newName: "Model");

            migrationBuilder.RenameColumn(
                name: "marca",
                table: "Carros",
                newName: "Brand");

            migrationBuilder.RenameColumn(
                name: "excluido",
                table: "Carros",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "ano",
                table: "Carros",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "AspNetUserTokens",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "AspNetUserTokens",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "login_provider",
                table: "AspNetUserTokens",
                newName: "LoginProvider");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AspNetUserTokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "AspNetUsers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AspNetUsers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "AspNetUsers",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "two_factor_enabled",
                table: "AspNetUsers",
                newName: "TwoFactorEnabled");

            migrationBuilder.RenameColumn(
                name: "security_stamp",
                table: "AspNetUsers",
                newName: "SecurityStamp");

            migrationBuilder.RenameColumn(
                name: "phone_number_confirmed",
                table: "AspNetUsers",
                newName: "PhoneNumberConfirmed");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "AspNetUsers",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "AspNetUsers",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "normalized_user_name",
                table: "AspNetUsers",
                newName: "NormalizedUserName");

            migrationBuilder.RenameColumn(
                name: "normalized_email",
                table: "AspNetUsers",
                newName: "NormalizedEmail");

            migrationBuilder.RenameColumn(
                name: "lockout_end",
                table: "AspNetUsers",
                newName: "LockoutEnd");

            migrationBuilder.RenameColumn(
                name: "lockout_enabled",
                table: "AspNetUsers",
                newName: "LockoutEnabled");

            migrationBuilder.RenameColumn(
                name: "email_confirmed",
                table: "AspNetUsers",
                newName: "EmailConfirmed");

            migrationBuilder.RenameColumn(
                name: "concurrency_stamp",
                table: "AspNetUsers",
                newName: "ConcurrencyStamp");

            migrationBuilder.RenameColumn(
                name: "access_failed_count",
                table: "AspNetUsers",
                newName: "AccessFailedCount");

            migrationBuilder.RenameColumn(
                name: "excluido",
                table: "AspNetUsers",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "AspNetUserRoles",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AspNetUserRoles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_asp_net_user_roles_role_id",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AspNetUserLogins",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "provider_display_name",
                table: "AspNetUserLogins",
                newName: "ProviderDisplayName");

            migrationBuilder.RenameColumn(
                name: "provider_key",
                table: "AspNetUserLogins",
                newName: "ProviderKey");

            migrationBuilder.RenameColumn(
                name: "login_provider",
                table: "AspNetUserLogins",
                newName: "LoginProvider");

            migrationBuilder.RenameIndex(
                name: "ix_asp_net_user_logins_user_id",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AspNetUserClaims",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AspNetUserClaims",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "claim_value",
                table: "AspNetUserClaims",
                newName: "ClaimValue");

            migrationBuilder.RenameColumn(
                name: "claim_type",
                table: "AspNetUserClaims",
                newName: "ClaimType");

            migrationBuilder.RenameIndex(
                name: "ix_asp_net_user_claims_user_id",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "AspNetRoles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AspNetRoles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "normalized_name",
                table: "AspNetRoles",
                newName: "NormalizedName");

            migrationBuilder.RenameColumn(
                name: "concurrency_stamp",
                table: "AspNetRoles",
                newName: "ConcurrencyStamp");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AspNetRoleClaims",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "AspNetRoleClaims",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "claim_value",
                table: "AspNetRoleClaims",
                newName: "ClaimValue");

            migrationBuilder.RenameColumn(
                name: "claim_type",
                table: "AspNetRoleClaims",
                newName: "ClaimType");

            migrationBuilder.RenameIndex(
                name: "ix_asp_net_role_claims_role_id",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Carros",
                table: "Carros",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Additionals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacidade_Carga = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacidade_Reboque = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacidade_Tanque = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Carro_id = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Tipo_Combustivel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Additionals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Additionals_Carros_Carro_id",
                        column: x => x.Carro_id,
                        principalTable: "Carros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consumes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Carro_id = table.Column<int>(type: "int", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estrada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consumes_Carros_Carro_id",
                        column: x => x.Carro_id,
                        principalTable: "Carros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dimensions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Altura = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Carro_id = table.Column<int>(type: "int", nullable: false),
                    Entre_Eixos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Largura = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dimensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dimensions_Carros_Carro_id",
                        column: x => x.Carro_id,
                        principalTable: "Carros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Modes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SavedModels",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Carro_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedModels", x => new { x.UserId, x.Carro_id });
                    table.ForeignKey(
                        name: "FK_SavedModels_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedModels_Carros_Carro_id",
                        column: x => x.Carro_id,
                        principalTable: "Carros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reliability = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Carro_id = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Potencia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rpm_potencia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rpm_torque = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Torque = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tracao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Transmissao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Specifications_Carros_Carro_id",
                        column: x => x.Carro_id,
                        principalTable: "Carros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aro = table.Column<int>(type: "int", nullable: false),
                    Carro_id = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Largura = table.Column<int>(type: "int", nullable: false),
                    Perfil = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tires_Carros_Carro_id",
                        column: x => x.Carro_id,
                        principalTable: "Carros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarroModes",
                columns: table => new
                {
                    Carro_id = table.Column<int>(type: "int", nullable: false),
                    ModeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarroModes", x => new { x.Carro_id, x.ModeId });
                    table.ForeignKey(
                        name: "FK_CarroModes_Carros_Carro_id",
                        column: x => x.Carro_id,
                        principalTable: "Carros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarroModes_Modes_ModeId",
                        column: x => x.ModeId,
                        principalTable: "Modes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdditionalSources",
                columns: table => new
                {
                    AdditionalId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    DataColeta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataReferencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalSources", x => new { x.AdditionalId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_AdditionalSources_Additionals_AdditionalId",
                        column: x => x.AdditionalId,
                        principalTable: "Additionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdditionalSources_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsumeSources",
                columns: table => new
                {
                    ConsumeId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    DataColeta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataReferencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumeSources", x => new { x.ConsumeId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_ConsumeSources_Consumes_ConsumeId",
                        column: x => x.ConsumeId,
                        principalTable: "Consumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumeSources_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DimensionSources",
                columns: table => new
                {
                    DimensionId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    DataColeta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataReferencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimensionSources", x => new { x.DimensionId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_DimensionSources_Dimensions_DimensionId",
                        column: x => x.DimensionId,
                        principalTable: "Dimensions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DimensionSources_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModeSources",
                columns: table => new
                {
                    ModeId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    DataColeta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataReferencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModeSourceModeId = table.Column<int>(type: "int", nullable: true),
                    ModeSourceSourceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeSources", x => new { x.ModeId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_ModeSources_ModeSources_ModeSourceModeId_ModeSourceSourceId",
                        columns: x => new { x.ModeSourceModeId, x.ModeSourceSourceId },
                        principalTable: "ModeSources",
                        principalColumns: new[] { "ModeId", "SourceId" });
                    table.ForeignKey(
                        name: "FK_ModeSources_Modes_ModeId",
                        column: x => x.ModeId,
                        principalTable: "Modes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModeSources_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SpecificationSources",
                columns: table => new
                {
                    SpecificationId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    DataColeta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataReferencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationSources", x => new { x.SpecificationId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_SpecificationSources_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpecificationSources_Specifications_SpecificationId",
                        column: x => x.SpecificationId,
                        principalTable: "Specifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TireSources",
                columns: table => new
                {
                    TireId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    DataColeta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataReferencia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TireSources", x => new { x.TireId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_TireSources_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TireSources_Tires_TireId",
                        column: x => x.TireId,
                        principalTable: "Tires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Additionals_Carro_id",
                table: "Additionals",
                column: "Carro_id");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalSources_SourceId",
                table: "AdditionalSources",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CarroModes_ModeId",
                table: "CarroModes",
                column: "ModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Consumes_Carro_id",
                table: "Consumes",
                column: "Carro_id");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumeSources_SourceId",
                table: "ConsumeSources",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Dimensions_Carro_id",
                table: "Dimensions",
                column: "Carro_id");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionSources_SourceId",
                table: "DimensionSources",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeSources_ModeSourceModeId_ModeSourceSourceId",
                table: "ModeSources",
                columns: new[] { "ModeSourceModeId", "ModeSourceSourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ModeSources_SourceId",
                table: "ModeSources",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedModels_Carro_id",
                table: "SavedModels",
                column: "Carro_id");

            migrationBuilder.CreateIndex(
                name: "IX_Specifications_Carro_id",
                table: "Specifications",
                column: "Carro_id");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationSources_SourceId",
                table: "SpecificationSources",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tires_Carro_id",
                table: "Tires",
                column: "Carro_id");

            migrationBuilder.CreateIndex(
                name: "IX_TireSources_SourceId",
                table: "TireSources",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
