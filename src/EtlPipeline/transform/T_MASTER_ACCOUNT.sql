CREATE OR ALTER VIEW [MAP].[T_MASTER_ACCOUNT] AS
SELECT
	A.Data_Folder_Id,
    A.Account,
	A.Account_Title,
	A.Account_Type,
    X.PrefixA,
    X.PrefixB,
    X.PrefixC,
    X.PrefixAB,
    X.PrefixABC,
    X.BaseAccount,
    X.Suffix,
	CASE WHEN X.Suffix = '' THEN X.BaseAccount ELSE X.BaseAccount + '.' + X.Suffix END AS ACCT
FROM [s300].[GLM_MASTER__ACCOUNT] A
JOIN [s300].[GLM_MASTER__ACCOUNT_FORMAT] AM
    ON AM.Data_Folder_Id = A.Data_Folder_Id
CROSS APPLY (
    SELECT
        LeftPart  = LEFT(A.Account, CHARINDEX('.', A.Account + '.') - 1),
        RightPart = SUBSTRING(A.Account, CHARINDEX('.', A.Account + '.') + 1, 8000)
) P
CROSS APPLY (
    SELECT
        -- cumulative prefixes (include hyphens because lengths include hyphens)
        PrefixA   = LEFT(P.LeftPart, AM.Account_Prefix_A_Length),
        PrefixAB  = LEFT(P.LeftPart, AM.Account_Prefix_AB_Length),
        PrefixABC = LEFT(P.LeftPart, AM.Account_Prefix_ABC_Length),

        -- segment B and C (skip the hyphen between segments)
        PrefixB = SUBSTRING(
                    P.LeftPart,
                    AM.Account_Prefix_A_Length + 2,
                    AM.Account_Prefix_AB_Length - AM.Account_Prefix_A_Length - 1
                  ),

        PrefixC = SUBSTRING(
                    P.LeftPart,
                    AM.Account_Prefix_AB_Length + 2,
                    AM.Account_Prefix_ABC_Length - AM.Account_Prefix_AB_Length - 1
                  ),

        -- base account: after the hyphen following PrefixABC
        BaseAccount = SUBSTRING(
                        P.LeftPart,
                        AM.Account_Prefix_ABC_Length + 2,
                        AM.Base_Account_Length
                      ),

        -- suffix: right side of decimal
        Suffix = RIGHT(P.RightPart, AM.Suffix_Length)
) X;
GO
