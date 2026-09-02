CREATE TABLE IF NOT EXISTS app."ApiRequestLog" (
    "Id"            BIGSERIAL    PRIMARY KEY,
    "CorrelationId" VARCHAR(64)  NOT NULL,
    "Method"        VARCHAR(10)  NOT NULL,
    "Path"          VARCHAR(500) NOT NULL,
    "StatusCode"    INTEGER      NOT NULL,
    "ElapsedMs"     BIGINT       NOT NULL,
    "Source"        VARCHAR(20)  NOT NULL,
    "CreatedOn"     TIMESTAMP    NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS "IX_ApiRequestLog_CorrelationId" ON app."ApiRequestLog"("CorrelationId");
CREATE INDEX IF NOT EXISTS "IX_ApiRequestLog_CreatedOn"     ON app."ApiRequestLog"("CreatedOn" DESC);
