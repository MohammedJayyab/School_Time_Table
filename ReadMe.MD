# When Can Overlapping Happen in the School Timetable?

Overlapping can occur in the school timetable when a teacher or classroom is assigned to multiple subjects at the same time slot. Here are the common cases:

## 1️⃣ Teacher Overlap (Same Teacher in Multiple Classes at the Same Time)

### ❌ Issue
- A teacher is assigned to two different classes for the same time period.

### 🔹 Example
- Monday, Period 2 → Teacher T1 is assigned to both C-10-G-a and C-11-S-a (impossible).

### ✅ Solution
- Ensure each teacher is only assigned to one class per period.
- Use a validation check before assigning a teacher.

## 2️⃣ Subject Overlap (Same Subject Assigned Multiple Times in a Day for a Class)

### ❌ Issue
- The same subject is repeated more than once in a single day for a class.

### 🔹 Example
- Monday, Period 1 → Math
- Monday, Period 4 → Math (Math should not repeat in one day).

### ✅ Solution
- Use a set (HashSet<string>) to track subjects for the day and prevent duplicates.

## 3️⃣ Classroom Overlap (Two Classes Assigned the Same Room at the Same Time)

### ❌ Issue
- If we have limited classrooms, two different classes might be assigned to the same room at the same time.

### 🔹 Example
- Room A is assigned to both C-10-G-a and C-11-S-b at Monday, Period 3.

### ✅ Solution
- Assign dedicated classrooms or check room availability before scheduling.

## 4️⃣ Teacher Workload Constraint (Teacher Assigned Too Many Periods Per Day/Week)

### ❌ Issue
- A teacher might be scheduled for too many classes per day, leading to overload.

### 🔹 Example
- Teacher T2 teaches 5 different periods on Monday (too much!).

### ✅ Solution
- Define maximum daily teaching hours for each teacher.

## ✅ Complete Smart Scheduling Strategy

### 💡 Before assigning a subject to a class, check these rules:

| ✅ Check | ❓ What to Validate? | 🚀 Fix Strategy |
|----------|-------------------|----------------|
| 1️⃣ Teacher Overlap | Is the teacher already assigned at this time? | Use Dictionary<string, HashSet<string>> (Day + Period → Teacher IDs) |
| 2️⃣ Subject Repetition | Is this subject already scheduled today? | Use Dictionary<string, HashSet<string>> (Day + ClassID → Subjects) |
| 3️⃣ Room Overlap | Is the classroom available? | Use Dictionary<string, HashSet<int>> (Day + Period → Room IDs) |
| 4️⃣ Teacher Overload | Has the teacher reached their daily limit? | Use Dictionary<string, int> (TeacherID → Assigned Count) |
