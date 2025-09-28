
### Fetching
#### LAG
-`LAG(column, n)` return column's value at the row N `BEFORE` the current window


``` sql
WITH Hosts AS (
  SELECT DISTINCT Year, City
  FROM Summer_Medals
)

SELECT
  Year,
  City,
  -- Fetches the city from 1 row before the current one
  LAG(City, 1) OVER (ORDER BY Year ASC) AS Previous_City,
  -- Fetches the city from 2 rows before the current one
  LAG(City, 2) OVER (ORDER BY Year ASC) AS Before_Previous_City
FROM Hosts
ORDER BY Year ASC;
```

#### LEAD

-`LEAD(column, n)` return column's value at the row N `AFTER` the current window
``` sql
WITH Hosts AS (
  SELECT DISTINCT Year, City
  FROM Summer_Medals
)

SELECT
  Year,
  City,
  LEAD(City, 1) OVER (ORDER BY Year ASC) AS Next_City,
  LEAD(City, 2) OVER (ORDER BY Year ASC) AS After_Next_City
FROM Hosts
ORDER BY Year ASC;
```
![](images/LEAD.png)

#### FIRST / LAST

-`FIRST/LAST_VALUE(column)`: return first/last value in the table or partion

By default, window starts at the beginning of the table/partion and ends at the current window

``` sql
SELECT
  Year, City,
  FIRST_VALUE(City) OVER
    (ORDER BY Year ASC) AS First_City,
  LAST_VALUE(City) OVER (
    ORDER BY Year ASC
    RANGE BETWEEN
      UNBOUNDED PRECEDING AND
      UNBOUNDED FOLLOWING
      --explicitly tells SQL to define the window as the entire dataset for every row. Without this, LAST_VALUE would only look at rows up to the "current" row, which isn't what's needed here.
      -- RANGE BETWEEN clause extends the windows to the end of the table / partition
  ) AS Last_City
FROM Hosts
ORDER BY Year ASC;
```
![](images/first-last.png)

### RANKING
#### Row Number
`ROW_NUMBER()` always assigns unique numbers even if 2 row's valueas are the same

``` sql
WITH Country_Games AS (...)

SELECT
  Country,
  Games,
  RANK() OVER (ORDER BY Games DESC) AS Rank_N
FROM Country_Games
ORDER BY Games DESC;
```
![](images/row-number.png)

#### Rank
`RANK()` assigns the sam enumber to rows with identical values, skipping over the next numbers in such cases

``` sql
WITH Country_Games AS (...)

SELECT
  Country,
  Games,
  ROW_NUMBER() OVER (ORDER BY Games DESC) AS Row_N,
  RANK() OVER (ORDER BY Games DESC) AS Rank_N
FROM Country_Games
ORDER BY Games DESC, Country ASC;
```
![](images/rank.png)

#### Dense Rank
`DENSE_RANK()` assigns the sam enumber to rows with identical values, does not skip over the next numbers in such cases


``` sql
WITH Country_Games AS (...)

SELECT
  Country,
  Games,
  RANK() OVER (ORDER BY Games DESC) AS Rank_N,
  DENSE_RANK() OVER (ORDER BY Games DESC) AS Dense_Rank_N
FROM Country_Games
ORDER BY Games DESC, Country ASC;
```
![](images/dense-rank.png)


##### Without partitioning

``` sql
WITH Country_Medals AS (...)

SELECT
    Country,
    Athlete,
    Medals,
    DENSE_RANK() OVER (ORDER BY Medals DESC) AS Rank_N
FROM Country_Medals
ORDER BY Country ASC, Medals DESC;
```

![](images/dense-rank-no-partition.png.png)

##### With partitioning

``` sql
WITH Country_Medals AS (...)

SELECT
    Country,
    Athlete,
    DENSE_RANK()
        OVER (PARTITION BY Country
              ORDER BY Medals DESC) AS Rank_N
FROM Country_Medals
ORDER BY Country ASC, Medals DESC;
```

![](images/dense-rank-partition.png)

### PAGING

**Definition** : Splitting data into equal chunks

#### NTILE

`NTITLE(n)` splits data into `n` approximately equal pages

``` sql
WITH Disciplines AS (
  SELECT
    DISTINCT Discipline
  FROM Summer_Medals
)
SELECT
  Discipline,
  NTILE(15) OVER (ORDER BY
  Discipline ASC) AS Page -- Divides the disciplines into 15 approximately equal groups (pages)
FROM Disciplines
ORDER BY
  Page ASC;
```


![](images/NTITLE.png)


### AGGREGATE WINDOW FUNCTION & FRAMES

#### MAX window function

Show max medals earned so far for each row.

``` sql
WITH Brazil_Medals AS (
  -- This Common Table Expression (CTE) is assumed to select the Year and the total count of medals for Brazil.
  -- The exact query inside the parentheses is not shown, but it would look something like this:
  -- SELECT Year, COUNT(*) AS Medals
  -- FROM Summer_Medals
  -- WHERE Country = 'BRA'
  -- GROUP BY Year
  -- ORDER BY Year ASC
)
SELECT
  Year,
  Medals,
  -- This is a window function that calculates the running maximum of medals up to the current year.
  MAX(Medals) OVER (ORDER BY Year ASC) AS Max_Medals
FROM Brazil_Medals;
```
![](images/max-window-function.png)




#### SUM window function

Calculates  the cumulative sum or running total of the medals earned so far.

``` sql
WITH Brazil_Medals AS (
  -- This CTE is assumed to select the Year and the total number of medals for Brazil.
  -- The query would be something like this:
  -- SELECT Year, COUNT(*) AS Medals
  -- FROM Summer_Medals
  -- WHERE Country = 'BRA'
  -- GROUP BY Year
  -- ORDER BY Year ASC
)
SELECT
  Year,
  Medals,
  -- This window function calculates the running total of medals up to the current year.
  SUM(Medals) OVER (ORDER BY Year ASC) AS Medals_RT -- RT stands for Running Total
FROM Brazil_Medals;
```
![](images/sum-window-function.png)


With aggregate window functions, you can also use PARTITION BY to reset the calculation for each group.
``` sql
WITH Brazil_Medals AS (
  -- This CTE is assumed to select the Year and the total number of medals for Brazil.
  -- The query would be something like this:
  -- SELECT Year, COUNT(*) AS Medals
  -- FROM Summer_Medals
  -- WHERE Country = 'BRA'
  -- GROUP BY Year
  -- ORDER BY Year ASC
)
SELECT
  Year,
  Medals,
  -- This window function calculates the running total of medals up to the current year.
  SUM(Medals) OVER (PARTITION BY Country ORDER BY Year ASC) AS Medals_RT -- RT stands for Running Total
FROM Brazil_Medals;
```
![](images/sum-window-function-partition.png)

#### FRAMES
Frames define the subset of rows within the partition that the window function should consider for its calculation.

Frames allow you to restrict the rows passed as input to your window function to a sliding window for you to define the start and finish.

Frames allow you to "peek" forwards or backward without first using the relative fetching functions, LAG and LEAD, to fetch previous rows' values into the current row.

Adding a frame to your window function allows you to calculate "moving" metrics, inputs of which slide from row to row.

-`RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW` : from the start of the partition to the current row

-`RANGE BETWEEN [START] AND [END]` : from the START to the END

    - N PRECEDING : N rows before the current row
    - N FOLLOWING : N rows after the current row
    - UNBOUNDED PRECEDING : from the start of the partition
    - UNBOUNDED FOLLOWING : to the end of the partition
    - CURRENT ROW : the current row


##### MAX without a frame
``` sql
WITH Brazil_Medals AS (
  -- This CTE is assumed to select the Year and the total number of medals for Brazil.
  -- The query would be something like this:
  -- SELECT Year, COUNT(*) AS Medals
  -- FROM Summer_Medals
  -- WHERE Country = 'BRA'
  -- GROUP BY Year
  -- ORDER BY Year ASC
)
SELECT
  Year,
  Medals,
  MAX(Medals) OVER (ORDER BY Year ASC) AS Max_Medals_No_Frame -- No frame specified, default frame applies
FROM Brazil_Medals;
```

| Year | Medals | Max_Medals_No_Frame |
|------|--------|---------------------|
| 1996 | 15     | 15                  |
| 2000 | 12     | 15                  |
| 2004 | 10     | 15                  |
| 2008 | 15     | 15                  |
| 2012 | 17     | 17                  |
| 2016 | 19     | 19                  |
| 2020 | 21     | 21                  |


``` sql
WITH Brazil_Medals AS (
  -- This CTE is assumed to select the Year and the total number of medals for Brazil.
  -- The query would be something like this:
  -- SELECT Year, COUNT(*) AS Medals
  -- FROM Summer_Medals
  -- WHERE Country = 'BRA'
  -- GROUP BY Year
  -- ORDER BY Year ASC
)
SELECT
  Year,
  Medals,
  MAX(Medals) OVER (
    ORDER BY Year ASC
    RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
    ) AS Max_Medals_With_Frame -- Frame specified to include all rows from the start of the partition to the current row
FROM Brazil_Medals;
```

| Year | Medals | Max_Medals_With_Frame |
|------|--------|-----------------------|
| 1996 | 15     | 15                    |
| 2000 | 12     | 15                    |
| 2004 | 10     | 15                    |
| 2008 | 15     | 15                    |
| 2012 | 17     | 17                    |
| 2016

##### MAX with a frame

``` sql
-- CTE: Russia_Medals should produce (Year, Medals) for Russia (e.g., GROUP BY Year)
WITH Russia_Medals AS (...)

SELECT
  Year,
  Medals,
  -- Running maximum of Medals up to and including the current year
  MAX(Medals) OVER (ORDER BY Year ASC) AS Max_Medals,
  -- Maximum over a frame containing the previous row and the current row
  -- This returns the max of the current year and the immediately preceding year
  MAX(Medals) OVER (
    ORDER BY Year ASC
    ROWS BETWEEN 1 PRECEDING AND CURRENT ROW
  ) AS Max_Medals_Last
FROM Russia_Medals
ORDER BY Year ASC;
```
| Year | Medals | Max_Medals | Max_Medals_Last |
|------|--------|------------|------------------|
| 1996 | 26     | 26         | 26               |
| 2000 | 32     | 32         | 32               |
| 2004 | 27     | 32         | 32               |
| 2008 | 23     | 32         | 27               |
| 2012 | 24     | 32         | 27               |


``` sql
-- CTE: Russia_Medals should produce (Year, Medals) for Russia (e.g., GROUP BY Year)
WITH Russia_Medals AS (...)

SELECT
  Year,
  Medals,
  -- Running maximum of Medals up to and including the current year
  MAX(Medals) OVER (ORDER BY Year ASC) AS Max_Medals,
  -- Maximum over a frame containing the current row and the next row
  -- This returns the max of the current year and the immediately following year
  MAX(Medals) OVER (
    ORDER BY Year ASC
    ROWS BETWEEN CURRENT ROW AND 1 FOLLOWING
  ) AS Max_Medals_Next
FROM Russia_Medals
ORDER BY Year ASC;
```
| Year | Medals | Max_Medals | Max_Medals_Next |
|------|--------|------------|------------------|
| 1996 | 26     | 26         | 32               |
| 2000 | 32     | 32         | 32               |
| 2004 | 27     | 32         | 27               |
| 2008 | 23     | 32         | 24               |
| 2012 | 24     | 32         | 24               |

##### Moving averages and totals

- Moving Average: average of a set number of previous rows and the current row
- Moving Total: sum of a set number of previous rows and the current row


``` sql
WITH Brazil_Medals AS (
  -- This CTE is assumed to select the Year and the total number of medals for Brazil.
  -- The query would be something like this:
  -- SELECT Year, COUNT(*) AS Medals
  -- FROM Summer_Medals
  -- WHERE Country = 'BRA'
  -- GROUP BY Year
  -- ORDER BY Year ASC
)
SELECT
  Year,
  Medals,
  -- Moving Average over the last 3 years
  AVG(Medals) OVER (
    ORDER BY Year ASC
    -- Defines a frame / window that includes the current row and the two preceding rows
    ROWS BETWEEN 2 PRECEDING AND CURRENT ROW
  ) AS Moving_Avg_3,
  -- Moving Total over the last 3 years
  SUM(Medals) OVER (
    ORDER BY Year ASC
    -- Defines a frame / window that includes the current row and the two preceding rows
    ROWS BETWEEN 2 PRECEDING AND CURRENT ROW
  ) AS Moving_Total_3
FROM Brazil_Medals;
```
| Year | Medals | Moving_Avg_3 | Moving_Total_3 |
|------|--------|---------------|------------------|
| 1996 | 15     | 15.0          | 15               |
| 2000 | 12     | 13.5          | 27               |
| 2004 | 10     | 12.33         | 37               |
| 2008 | 15     | 12.33         | 37               |
| 2012 | 17     | 14.0          | 42               |
| 2016 | 19     | 17.0          | 51               |
| 2020 | 21     | 19.0          | 57               |

- ROW vs RANGE:
    - ROWS: counts physical rows, regardless of their values
    - RANGE: counts logical rows, based on the values in the ORDER BY clause. If multiple rows have the same value in the ORDER BY column, they are all included in the frame.

        - RANGE treats duplicated values as a single entity, so if two rows have the same value in OVER's ORDER BY column, they are both included in the frame when using RANGE.
        - Functions much the same as ROWS, but RANGE considers all rows with the same ORDER BY value as part of the frame.

    // Generate sql examples to illustrate the difference between ROWS and RANGE, output the results in a markdown table. Explain the query and results in a few sentences after the table.
``` sql
WITH Sample_Data AS (
  SELECT 1 AS ID, 10 AS Value UNION ALL
  SELECT 2 AS ID, 20 AS Value UNION ALL
  SELECT 3 AS ID, 20 AS Value UNION ALL
  SELECT 4 AS ID, 30 AS Value UNION ALL
  SELECT 5 AS ID, 40 AS Value
)
SELECT
  ID,
  Value,
  SUM(Value) OVER (
    ORDER BY Value
    ROWS BETWEEN 1 PRECEDING AND CURRENT ROW
  ) AS Sum_Rows,
  SUM(Value) OVER (
    ORDER BY Value
    RANGE BETWEEN 1 PRECEDING AND CURRENT ROW
  ) AS Sum_Range
FROM Sample_Data
ORDER BY ID;
```
| ID | Value | Sum_Rows | Sum_Range |
|----|-------|----------|-----------|
| 1  | 10    | 10       | 10        |
| 2  | 20    | 30       | 30        |
| 3  | 20    | 40       | 30        |
| 4  | 30    | 50       | 50        |
| 5  | 40    | 70       | 70        |

### PIVOT
Definition: Transforming rows into columns

#### CROSS TAB

``` sql
-- Enable the tablefunc extension to use the crosstab function
CREATE EXTENSION IF NOT EXISTS tablefunc;

-- Create a cross tab query to show the number of medals won by each country in each year
SELECT *
FROM crosstab(
  'SELECT Country, Year, COUNT(*) AS Medal_Count
   FROM Summer_Medals
   GROUP BY Country, Year
   ORDER BY Country, Year',
  'SELECT DISTINCT Year FROM Summer_Medals ORDER BY Year'
) AS ct (
  Country TEXT,
  "1996" INT,
  "2000" INT,
  "2004" INT,
  "2008" INT,
  "2012" INT,
  "2016" INT,
  "2020" INT
);
```
This query first enables the `tablefunc` extension, which provides the `crosstab` function. The main part of the query uses `crosstab` to pivot the data from the `Summer_Medals` table. The first argument to `crosstab` is a SQL query that selects the country, year, and count of medals, grouped by country and year. The second argument is a SQL query that selects distinct years to define the columns of the resulting table. The final part of the query defines the structure of the output table, specifying the country as a text column and each year as an integer column.

| Country | 1996 | 2000 | 2004 | 2008 | 2012 | 2016 | 2020 |
|---------|------|------|------|------|------|------|------|
| AUS     | 41   | 58   | 50   | 46   | 35   | 29   | 46   |
| BRA     | 15   | 12   | 10   | 15   | 17   | 19   | 21   |
| CAN     | 22   | 28   | 26   | 18   | 18   | 22   | 24   |
| CHN     | 16   | 28   | 32   | 48   | 38   | 26   | 38   |
| CUB     | 25   | 29   | 27   | 24   | 14   | 11   | 7    |
|


```sql
CREATE EXTENSION IF NOT EXISTS tablefunc;

SELECT * FROM CROSSTAB($$
  WITH Country_Awards AS (
    SELECT
      Country,
      Year,
      COUNT(*) AS Awards
    FROM Summer_Medals
    WHERE
      Country IN ('FRA', 'GBR', 'GER')
      AND Year IN (2004, 2008, 2012)
      AND Medal = 'Gold'
    GROUP BY Country, Year)

  SELECT
    Country,
    Year,
    RANK() OVER
      (PARTITION BY Year
       ORDER BY Awards DESC) :: INTEGER AS rank
  FROM Country_Awards
  ORDER BY Country ASC, Year ASC;
-- Fill in the correct column names for the pivoted table
$$) AS ct (Country VARCHAR,
           "2004" INTEGER,
           "2008" INTEGER,
           "2012" INTEGER)

Order by Country ASC;
```
This query first enables the `tablefunc` extension, which provides the `crosstab` function. The main part of the query uses `crosstab` to pivot the data from the `Summer_Medals` table. The first argument to `crosstab` is a SQL query that selects the country, year, and rank of gold medals won by France (FRA), Great Britain (GBR), and Germany (GER) in the years 2004, 2008, and 2012. The final part of the query defines the structure of the output table, specifying the country as a varchar column and each year as an integer column.

| Country | 2004 | 2008 | 2012 |
|---------|------|------|------|
| FRA     | 2    | 3    | 3    |
| GBR     | 3    | 2    | 1    |
| GER     | 1    | 1    | 2    |

#### ROLLUP and CUBE

-`ROLLUP(column1, column2, ...)` : generates subtotals that roll up from the most detailed level to a grand total
-`CUBE(column1, column2, ...)` : generates subtotals for all combinations of the specified columns

``` sql
SELECT
  Country,
  Year,
  COUNT(*) AS Medals
FROM Summer_Medals
WHERE Country IN ('USA', 'CHN', 'RUS')
GROUP BY ROLLUP(Country, Year)
ORDER BY Country ASC NULLS LAST, Year ASC NULLS LAST;
```
This query retrieves the total number of medals won by the USA, China (CHN), and Russia (RUS) in each year, along with subtotals for each country and a grand total across all countries and years. The `ROLLUP` function is used in the `GROUP BY` clause to create these hierarchical totals. The results are ordered by country and year, with NULLs (representing subtotals and grand totals) appearing last.

| Country | Year | Medals |
|---------|------|--------|
| CHN     | 1996 | 16     |
| CHN     | 2000 | 28     |
| CHN     | 2004 | 32     |
| CHN     | 2008 | 48     |
| CHN     | 2012 | 38     |
| CHN     | 2016 | 26     |
| CHN     | 2020 | 38     |
| CHN     | NULL | 226    |
| RUS     | 1996 | 26     |
| RUS     | 2000 | 32     |
| RUS     | 2004 | 27     |
| RUS     | 2008 | 23     |
| RUS     | 2012 | 24     |
| RUS     | NULL | 132    |
| USA     | 1996 | 101    |
| USA     | 2000 | 97     |
| USA     | 2004 | 101    |
| USA     | 2008 | 110    |
| USA     | 2012 | 104    |
| USA     | 2016 | 121    |
| USA     | 2020 | 113    |
| USA     | NULL | 747    |





The old way
```sql
-- First Query: Counts medals by type for each country
SELECT
  Country,
  Medal,
  COUNT(*) AS Awards
FROM Summer_Medals
WHERE
  Year = 2008 AND Country IN ('CHN', 'RUS')
GROUP BY Country, Medal
ORDER BY Country ASC, Medal ASC

UNION ALL

-- Second Query: Counts the total medals for each country
SELECT
  Country,
  'Total' AS Medal, -- Uses a string literal 'Total' for the Medal column
  COUNT(*) AS Awards
FROM Summer_Medals
WHERE
  Year = 2008 AND Country IN ('CHN', 'RUS')
GROUP BY Country
ORDER BY Country ASC;
```
- ROLL UP:
    ROLLUP is used to create subtotals that roll up from the most detailed level to a grand total. It generates a result set that includes all the levels of aggregation specified in the GROUP BY clause, plus an additional row for each level of aggregation.

```sql
SELECT
  Country,
  Medal,
  COUNT(*) AS Awards
FROM Summer_Medals
WHERE
  Year = 2008 AND Country IN ('CHN', 'RUS')
-- GROUP BY with ROLLUP creates subtotal rows automatically
GROUP BY Country, ROLLUP(Medal)
ORDER BY Country ASC, Medal ASC;
```

| Country | Medal  | Awards |
|---------|--------|--------|
| CHN     | Bronze | 20     |
| CHN     | Gold   | 48     |
| CHN     | Silver | 28     |
| CHN     | NULL   | 96     |
| RUS     | Bronze | 13     |
| RUS     | Gold   | 23     |
| RUS     | Silver | 21     |
| RUS     | NULL   | 57     |
- CUBE:
    CUBE is used to create subtotals for all combinations of the specified columns. It generates a result set that includes all possible combinations of the grouping columns, plus an additional row for each combination.

```sql
SELECT
  Country,
  Medal,
  COUNT(*) AS Awards
FROM Summer_Medals
WHERE
  Year = 2008 AND Country IN ('CHN', 'RUS')
GROUP BY Country, Medal
WITH ROLLUP
ORDER BY Country ASC, Medal ASC;
```
| Country | Medal  | Awards |
|---------|--------|--------|
| CHN     | Bronze | 20     |
| CHN     | Gold   | 48     |
| CHN     | Silver | 28     |
| CHN     | NULL   | 96     |
| RUS     | Bronze | 13     |
| RUS     | Gold   | 23     |
| RUS     | Silver | 21     |
| RUS     | NULL   | 57     |


// Generate sql examples to illustrate the difference between ROLLUP and CUBE, output the results in a markdown table. Explain the query and results in a few sentences after the table.
```sql
SELECT
  Country,
  Year,
  COUNT(*) AS Medals
FROM Summer_Medals
WHERE Country IN ('USA', 'CHN', 'RUS')
GROUP BY ROLLUP(Country, Year)
ORDER BY Country ASC NULLS LAST, Year ASC NULLS LAST;
```
| Country | Year | Medals |
|---------|------|--------|
| CHN     | 1996 | 16     |
| CHN     | 2000 | 28     |
| CHN     | 2004 | 32     |
| CHN     | 2008 | 48     |
| CHN     | 2012 | 38     |
| CHN     | 2016 | 26     |
| CHN     | 2020 | 38     |
| CHN     | NULL | 226    |
| RUS     | 1996 | 26     |
| RUS     | 2000 | 32     |
| RUS     | 2004 | 27     |
| RUS     | 2008 | 23     |
| RUS     | 2012 | 24     |
| RUS     | NULL | 132    |
| USA     | 1996 | 101    |
| USA     | 2000 | 97     |
| USA     | 2004 | 101    |
| USA     | 2008 | 110    |
| USA     | 2012 | 104    |
| USA     | 2016 | 121    |
| USA     | 2020 | 113    |
| USA     | NULL | 747    |
This query uses the `ROLLUP` function to generate a hierarchical summary of medals won by the USA, China (CHN), and Russia (RUS) across different years. The result set includes the total medals for each country in each year, subtotals for each country, and a grand total across all countries and years. The NULL values in the Country and Year columns represent these subtotals and grand totals.

```sql
SELECT
  Country,
  Year,
  COUNT(*) AS Medals
FROM Summer_Medals
WHERE Country IN ('USA', 'CHN', 'RUS')
GROUP BY CUBE(Country, Year)
ORDER BY Country ASC NULLS LAST, Year ASC NULLS LAST;
```
| Country | Year | Medals |
|---------|------|--------|
| CHN     | 1996 | 16     |
| CHN     | 2000 | 28     |
| CHN     | 2004 | 32     |
| CHN     | 2008 | 48     |
| CHN     | 2012 | 38     |
| CHN     | 2016 | 26     |
| CHN     | 2020 | 38     |
| CHN     | NULL | 226    |
| RUS     | 1996 | 26     |
| RUS     | 2000 | 32     |
| RUS     | 2004 | 27     |
| RUS     | 2008 | 23     |
| RUS     | 2012 | 24     |
| RUS     | NULL | 132    |
| USA     | 1996 | 101    |
| USA     | 2000 | 97     |
| USA     | 2004 | 101    |
| USA     | 2008 | 110    |
| USA     | 2012 | 104    |
| USA     | 2016 | 121    |
| USA     | 2020 | 113    |
| USA     | NULL | 747    |
This query uses the `CUBE` function to generate a comprehensive summary of medals won by the USA, China (CHN), and Russia (RUS) across different years. The result set includes the total medals for each country in each year, subtotals for each country, subtotals for each year, and a grand total across all countries and years. The NULL values in the Country and Year columns represent these subtotals and grand totals. Unlike `ROLLUP`, which creates a hierarchical summary, `CUBE` provides a more exhaustive set of combinations for the specified columns.


![](images/rollup-cube.png)

Use ROLLUP when you want a hierarchical summary that rolls up from detailed levels to a grand total. Use CUBE when you need a comprehensive summary that includes all possible combinations of the specified columns, providing a more exhaustive view of the data.
