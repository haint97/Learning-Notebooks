
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
