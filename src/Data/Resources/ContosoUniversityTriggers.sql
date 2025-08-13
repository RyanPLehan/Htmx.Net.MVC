BEGIN TRANSACTION;

CREATE TRIGGER IF NOT EXISTS "TRG_Department_Insert"
AFTER INSERT ON Department
BEGIN
	UPDATE	Department
	SET		"RowVersion" = randomblob(8)
	WHERE	rowid = NEW.rowid;
END;


CREATE TRIGGER IF NOT EXISTS "TRG_Department_Update"
AFTER UPDATE ON Department
FOR EACH ROW
BEGIN
	UPDATE	Department
	SET		"RowVersion" = randomblob(8)
	WHERE	rowid = OLD.rowid;
END;

COMMIT;
