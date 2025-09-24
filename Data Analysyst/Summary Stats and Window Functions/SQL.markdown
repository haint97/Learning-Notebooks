
### Fetching
#### LAG
-`LAG(column, n)` return column's value at the row N `BEFORE` the current window

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
