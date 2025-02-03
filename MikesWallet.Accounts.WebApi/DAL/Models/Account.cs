using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MikesWallet.Accounts.WebApi.DAL.Models;

public class Account
{
    public required Guid Id { get; init; }
    
    public required Guid UserId { get; init; }
    
    public required string Name { get; init; }
    
    public required DateTimeOffset CreationDateTime { get; init; }
    
    public required Currency Currency { get; init; }
    
    /*
     Normally account balance should be calculated with addition/subtraction operations related to this account.
     To improve performance(read) this column will store actual balance which I will change all the time the new operation created. 
    */
    public required decimal Balance { get; set; }
}

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.Property(e => e.Name);
        
        builder
            .Property(e => e.Currency)
            .HasConversion(e => e.ToString(), e => Enum.Parse<Currency>(e));

        builder
            .HasIndex(e => new { e.UserId, e.Currency })
            .IsUnique();
        
        builder
            .Property(e => e.Balance)
            .HasPrecision(19, 2);

        builder.ToTable(t => t.HasCheckConstraint(
            $"CK_{nameof(Account)}_{nameof(Account.Balance)}", 
            $"\"{nameof(Account.Balance)}\" >= 0"));
    }
}

// ReSharper disable InconsistentNaming
public enum Currency
{
    [Display(Name = "UAE Dirham")]
    AED,

    [Display(Name = "Afghan Afghani")]
    AFN,

    [Display(Name = "Albanian Lek")]
    ALL,

    [Display(Name = "Armenian Dram")]
    AMD,

    [Display(Name = "Netherlands Antillian Guilder")]
    ANG,

    [Display(Name = "Angolan Kwanza")]
    AOA,

    [Display(Name = "Argentine Peso")]
    ARS,

    [Display(Name = "Australian Dollar")]
    AUD,

    [Display(Name = "Aruban Florin")]
    AWG,

    [Display(Name = "Azerbaijani Manat")]
    AZN,

    [Display(Name = "Bosnia and Herzegovina Mark")]
    BAM,

    [Display(Name = "Barbados Dollar")]
    BBD,

    [Display(Name = "Bangladeshi Taka")]
    BDT,

    [Display(Name = "Bulgarian Lev")]
    BGN,

    [Display(Name = "Bahraini Dinar")]
    BHD,

    [Display(Name = "Burundian Franc")]
    BIF,

    [Display(Name = "Bermudian Dollar")]
    BMD,

    [Display(Name = "Brunei Dollar")]
    BND,

    [Display(Name = "Bolivian Boliviano")]
    BOB,

    [Display(Name = "Brazilian Real")]
    BRL,

    [Display(Name = "Bahamian Dollar")]
    BSD,

    [Display(Name = "Bhutanese Ngultrum")]
    BTN,

    [Display(Name = "Botswana Pula")]
    BWP,

    [Display(Name = "Belarusian Ruble")]
    BYN,

    [Display(Name = "Belize Dollar")]
    BZD,

    [Display(Name = "Canadian Dollar")]
    CAD,

    [Display(Name = "Congolese Franc")]
    CDF,

    [Display(Name = "Swiss Franc")]
    CHF,

    [Display(Name = "Chilean Peso")]
    CLP,

    [Display(Name = "Chinese Renminbi")]
    CNY,

    [Display(Name = "Colombian Peso")]
    COP,

    [Display(Name = "Costa Rican Colon")]
    CRC,

    [Display(Name = "Cuban Peso")]
    CUP,

    [Display(Name = "Cape Verdean Escudo")]
    CVE,

    [Display(Name = "Czech Koruna")]
    CZK,

    [Display(Name = "Djiboutian Franc")]
    DJF,

    [Display(Name = "Danish Krone")]
    DKK,

    [Display(Name = "Dominican Peso")]
    DOP,

    [Display(Name = "Algerian Dinar")]
    DZD,

    [Display(Name = "Egyptian Pound")]
    EGP,

    [Display(Name = "Eritrean Nakfa")]
    ERN,

    [Display(Name = "Ethiopian Birr")]
    ETB,

    [Display(Name = "Euro")]
    EUR,

    [Display(Name = "Fiji Dollar")]
    FJD,

    [Display(Name = "Falkland Islands Pound")]
    FKP,

    [Display(Name = "Faroese Króna")]
    FOK,

    [Display(Name = "Pound Sterling")]
    GBP,

    [Display(Name = "Georgian Lari")]
    GEL,

    [Display(Name = "Guernsey Pound")]
    GGP,

    [Display(Name = "Ghanaian Cedi")]
    GHS,

    [Display(Name = "Gibraltar Pound")]
    GIP,

    [Display(Name = "Gambian Dalasi")]
    GMD,

    [Display(Name = "Guinean Franc")]
    GNF,

    [Display(Name = "Guatemalan Quetzal")]
    GTQ,

    [Display(Name = "Guyanese Dollar")]
    GYD,

    [Display(Name = "Hong Kong Dollar")]
    HKD,

    [Display(Name = "Honduran Lempira")]
    HNL,

    [Display(Name = "Croatian Kuna")]
    HRK,

    [Display(Name = "Haitian Gourde")]
    HTG,

    [Display(Name = "Hungarian Forint")]
    HUF,

    [Display(Name = "Indonesian Rupiah")]
    IDR,

    [Display(Name = "Israeli New Shekel")]
    ILS,

    [Display(Name = "Manx Pound")]
    IMP,

    [Display(Name = "Indian Rupee")]
    INR,

    [Display(Name = "Iraqi Dinar")]
    IQD,

    [Display(Name = "Iranian Rial")]
    IRR,

    [Display(Name = "Icelandic Króna")]
    ISK,

    [Display(Name = "Jersey Pound")]
    JEP,

    [Display(Name = "Jamaican Dollar")]
    JMD,

    [Display(Name = "Jordanian Dinar")]
    JOD,

    [Display(Name = "Japanese Yen")]
    JPY,

    [Display(Name = "Kenyan Shilling")]
    KES,

    [Display(Name = "Kyrgyzstani Som")]
    KGS,

    [Display(Name = "Cambodian Riel")]
    KHR,

    [Display(Name = "Kiribati Dollar")]
    KID,

    [Display(Name = "Comorian Franc")]
    KMF,

    [Display(Name = "South Korean Won")]
    KRW,

    [Display(Name = "Kuwaiti Dinar")]
    KWD,

    [Display(Name = "Cayman Islands Dollar")]
    KYD,

    [Display(Name = "Kazakhstani Tenge")]
    KZT,

    [Display(Name = "Lao Kip")]
    LAK,

    [Display(Name = "Lebanese Pound")]
    LBP,

    [Display(Name = "Sri Lanka Rupee")]
    LKR,

    [Display(Name = "Liberian Dollar")]
    LRD,

    [Display(Name = "Lesotho Loti")]
    LSL,

    [Display(Name = "Libyan Dinar")]
    LYD,

    [Display(Name = "Moroccan Dirham")]
    MAD,

    [Display(Name = "Moldovan Leu")]
    MDL,

    [Display(Name = "Malagasy Ariary")]
    MGA,

    [Display(Name = "Macedonian Denar")]
    MKD,

    [Display(Name = "Burmese Kyat")]
    MMK,

    [Display(Name = "Mongolian Tögrög")]
    MNT,

    [Display(Name = "Macanese Pataca")]
    MOP,

    [Display(Name = "Mauritanian Ouguiya")]
    MRU,

    [Display(Name = "Mauritian Rupee")]
    MUR,

    [Display(Name = "Maldivian Rufiyaa")]
    MVR,

    [Display(Name = "Malawian Kwacha")]
    MWK,

    [Display(Name = "Mexican Peso")]
    MXN,

    [Display(Name = "Malaysian Ringgit")]
    MYR,

    [Display(Name = "Mozambican Metical")]
    MZN,

    [Display(Name = "Namibian Dollar")]
    NAD,

    [Display(Name = "Nigerian Naira")]
    NGN,

    [Display(Name = "Nicaraguan Córdoba")]
    NIO,

    [Display(Name = "Norwegian Krone")]
    NOK,

    [Display(Name = "Nepalese Rupee")]
    NPR,

    [Display(Name = "New Zealand Dollar")]
    NZD,

    [Display(Name = "Omani Rial")]
    OMR,

    [Display(Name = "Panamanian Balboa")]
    PAB,

    [Display(Name = "Peruvian Sol")]
    PEN,

    [Display(Name = "Papua New Guinean Kina")]
    PGK,

    [Display(Name = "Philippine Peso")]
    PHP,

    [Display(Name = "Pakistani Rupee")]
    PKR,

    [Display(Name = "Polish Złoty")]
    PLN,
    
    [Display(Name = "United States Dollar")]
    USD,

    [Display(Name = "Ukrainian Hryvnia")]
    UAH,

    [Display(Name = "Russian Ruble")]
    RUB,
}
