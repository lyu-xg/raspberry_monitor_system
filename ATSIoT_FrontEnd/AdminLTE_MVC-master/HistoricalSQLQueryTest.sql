SELECT sensorID, timestamp, value, dataType, serverTime
                    FROM dbo.serverTime WHERE timestamp>'2016-05-31T06:30:01Z' AND timestamp<'2016-05-31T12:00:01Z' AND dataType='te';  -- In TestingSQLDatabase database.
                    