CREATE OR ALTER VIEW [MAP].[T_MASTER_EMPLOYEE]
AS
SELECT
    E.*,
    N.LAST_NAME,
    N.FIRST_NAME,
    N.MI
FROM [s300].[PRM_MASTER__EMPLOYEE] E
CROSS APPLY (
    SELECT
        DelimPos =
            CASE
                WHEN CHARINDEX(';', E.EMPLOYEE_NAME) > 0 THEN CHARINDEX(';', E.EMPLOYEE_NAME)
                WHEN CHARINDEX(',', E.EMPLOYEE_NAME) > 0 THEN CHARINDEX(',', E.EMPLOYEE_NAME)
                ELSE 0
            END
) D
CROSS APPLY (
    SELECT
        LastNameRaw =
            CASE
                WHEN D.DelimPos > 0 THEN LEFT(E.EMPLOYEE_NAME, D.DelimPos - 1)
                ELSE LTRIM(RTRIM(E.EMPLOYEE_NAME))
            END,
        RemainderRaw =
            CASE
                WHEN D.DelimPos > 0 THEN LTRIM(RTRIM(SUBSTRING(E.EMPLOYEE_NAME, D.DelimPos + 1, LEN(E.EMPLOYEE_NAME))))
                ELSE ''
            END
) P
CROSS APPLY (
    SELECT
        SpacePos = CHARINDEX(' ', P.RemainderRaw)
) S
CROSS APPLY (
    SELECT
        LAST_NAME  = NULLIF(LTRIM(RTRIM(P.LastNameRaw)), ''),
        FIRST_NAME =
            CASE
                WHEN P.RemainderRaw = '' THEN ''
                WHEN S.SpacePos = 0 THEN P.RemainderRaw
                ELSE LEFT(P.RemainderRaw, S.SpacePos - 1)
            END,
        MI =
            CASE
                WHEN P.RemainderRaw = '' THEN ''
                WHEN S.SpacePos = 0 THEN ''
                ELSE LTRIM(RTRIM(SUBSTRING(P.RemainderRaw, S.SpacePos + 1, LEN(P.RemainderRaw))))
            END
) N;
GO
