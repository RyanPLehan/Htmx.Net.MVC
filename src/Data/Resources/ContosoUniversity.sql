BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "Person" (
	"ID"	INTEGER NOT NULL,
	"Discriminator"	TEXT NOT NULL,
	"FirstName"	TEXT NOT NULL,
	"LastName"	TEXT NOT NULL,
	"HireDate"	TEXT,
	"EnrollmentDate"	TEXT,
	CONSTRAINT "PK_Person" PRIMARY KEY("ID" AUTOINCREMENT)
);

CREATE TABLE IF NOT EXISTS "Department" (
	"DepartmentID"	INTEGER NOT NULL,
	"Budget"	money NOT NULL,
	"InstructorID"	INTEGER,
	"Name"	TEXT,
	"RowVersion"	BLOB,
	"StartDate"	TEXT NOT NULL,
	CONSTRAINT "PK_Department" PRIMARY KEY("DepartmentID" AUTOINCREMENT),
	CONSTRAINT "FK_Department_Person_InstructorID" FOREIGN KEY("InstructorID") REFERENCES "Person"("ID") ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS "OfficeAssignment" (
	"InstructorID"	INTEGER NOT NULL,
	"Location"	TEXT,
	CONSTRAINT "PK_OfficeAssignment" PRIMARY KEY("InstructorID"),
	CONSTRAINT "FK_OfficeAssignment_Person_InstructorID" FOREIGN KEY("InstructorID") REFERENCES "Person"("ID") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Course" (
	"CourseID"	INTEGER NOT NULL,
	"Credits"	INTEGER NOT NULL,
	"DepartmentID"	INTEGER NOT NULL,
	"Title"	TEXT,
	CONSTRAINT "PK_Course" PRIMARY KEY("CourseID"),
	CONSTRAINT "FK_Course_Department_DepartmentID" FOREIGN KEY("DepartmentID") REFERENCES "Department"("DepartmentID") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "CourseAssignment" (
	"CourseID"	INTEGER NOT NULL,
	"InstructorID"	INTEGER NOT NULL,
	CONSTRAINT "PK_CourseAssignment" PRIMARY KEY("CourseID","InstructorID"),
	CONSTRAINT "FK_CourseAssignment_Course_CourseID" FOREIGN KEY("CourseID") REFERENCES "Course"("CourseID") ON DELETE CASCADE,
	CONSTRAINT "FK_CourseAssignment_Person_InstructorID" FOREIGN KEY("InstructorID") REFERENCES "Person"("ID") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Enrollment" (
	"EnrollmentID"	INTEGER NOT NULL,
	"CourseID"	INTEGER NOT NULL,
	"Grade"	INTEGER,
	"StudentID"	INTEGER NOT NULL,
	CONSTRAINT "PK_Enrollment" PRIMARY KEY("EnrollmentID" AUTOINCREMENT),
	CONSTRAINT "FK_Enrollment_Person_StudentID" FOREIGN KEY("StudentID") REFERENCES "Person"("ID") ON DELETE CASCADE,
	CONSTRAINT "FK_Enrollment_Course_CourseID" FOREIGN KEY("CourseID") REFERENCES "Course"("CourseID") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_Course_DepartmentID" ON "Course" (
	"DepartmentID"
);
CREATE INDEX IF NOT EXISTS "IX_CourseAssignment_InstructorID" ON "CourseAssignment" (
	"InstructorID"
);
CREATE INDEX IF NOT EXISTS "IX_Department_InstructorID" ON "Department" (
	"InstructorID"
);
CREATE INDEX IF NOT EXISTS "IX_Enrollment_CourseID" ON "Enrollment" (
	"CourseID"
);
CREATE INDEX IF NOT EXISTS "IX_Enrollment_StudentID" ON "Enrollment" (
	"StudentID"
);

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
