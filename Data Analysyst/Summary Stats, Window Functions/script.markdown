
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
