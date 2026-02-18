--STATE DEFINITIONS
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'MAP'
      AND TABLE_NAME   = 'T_STATE'
)
BEGIN
    CREATE TABLE MAP.T_STATE
    (
        STATE_ID   VARCHAR(5)  NOT NULL,
        STATE_NAME VARCHAR(100) NOT NULL,
        CONSTRAINT PK_T_STATE PRIMARY KEY (STATE_ID)
    );
    INSERT INTO MAP.T_STATE (STATE_ID, STATE_NAME)
VALUES
    ('AB', 'Alberta'),
    ('AK', 'Alaska'),
    ('AL', 'Alabama'),
    ('AR', 'Arkansas'),
    ('AS', 'American Samoa'),
    ('AZ', 'Arizona'),
    ('BA', 'Bahamas'),
    ('BC', 'British Columbia'),
    ('CA', 'California'),
    ('CO', 'Colorado'),
    ('CT', 'Connecticut'),
    ('DC', 'District of Columbia'),
    ('DE', 'Delaware'),
    ('FL', 'Florida'),
    ('GA', 'Georgia'),
    ('GU', 'Guam'),
    ('HI', 'Hawaii'),
    ('IA', 'Iowa'),
    ('ID', 'Idaho'),
    ('IL', 'Illinois'),
    ('IN', 'Indiana'),
    ('KS', 'Kansas'),
    ('KY', 'Kentucky'),
    ('LA', 'Louisiana'),
    ('MA', 'Massachusetts'),
    ('MB', 'Manitoba'),
    ('MD', 'Maryland'),
    ('ME', 'Maine'),
    ('MI', 'Michigan'),
    ('MN', 'Minnesota'),
    ('MO', 'Missouri'),
    ('MS', 'Mississippi'),
    ('MT', 'Montana'),
    ('NB', 'New Brunswick'),
    ('NC', 'North Carolina'),
    ('ND', 'North Dakota'),
    ('NE', 'Nebraska'),
    ('NH', 'New Hampshire'),
    ('NJ', 'New Jersey'),
    ('NL', 'Newfoundland'),
    ('NM', 'New Mexico'),
    ('NS', 'Nova Scotia'),
    ('NT', 'Northwest Territory'),
    ('NU', 'Nunavut'),
    ('NV', 'Nevada'),
    ('NY', 'New York'),
    ('OH', 'Ohio'),
    ('OK', 'Oklahoma'),
    ('ON', 'Ontario'),
    ('OR', 'Oregon'),
    ('PA', 'Pennsylvania'),
    ('PE', 'Prince Edward Island'),
    ('PR', 'Puerto Rico'),
    ('QC', 'Province of Quebec'),
    ('RI', 'Rhode Island'),
    ('SC', 'South Carolina'),
    ('SD', 'South Dakota'),
    ('SK', 'Saskatchewan'),
    ('TN', 'Tennessee'),
    ('TX', 'Texas'),
    ('UT', 'Utah'),
    ('VA', 'Virginia'),
    ('VI', 'Virgin Islands'),
    ('VT', 'Vermont'),
    ('WA', 'Washington'),
    ('WI', 'Wisconsin'),
    ('WV', 'West Virginia'),
    ('WY', 'Wyoming'),
    ('YT', 'Yukon Territory'),
    ('',  '');
END;
GO
