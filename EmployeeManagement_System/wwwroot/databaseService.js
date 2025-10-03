class DatabaseService {
    constructor() {
        this.baseUrl = '/api'; // This would be your backend API endpoint
    }

    // Simulated API calls - in a real app, these would be fetch/axios calls to your C# backend
    async Signup(username, password, email) {
        // Simulate API call delay
        await this.simulateDelay();

        // In a real implementation, this would be:
        // const response = await fetch('/api/auth/signup', {
        //     method: 'POST',
        //     headers: { 'Content-Type': 'application/json' },
        //     body: JSON.stringify({ username, password, email })
        // });
        // return await response.text();

        // Simulated response
        const users = this.getStoredUsers();
        if (users.find(u => u.username === username)) {
            return "ERROR: Username already exists";
        }
        if (users.find(u => u.email === email)) {
            return "ERROR: Email already registered";
        }

        const newUser = {
            id: users.length + 1,
            username,
            password: this.simulateHash(password),
            email
        };
        users.push(newUser);
        this.setStoredUsers(users);

        // Set current user
        localStorage.setItem('currentUser', JSON.stringify(newUser));

        return "SUCCESS: User registered successfully";
    }

    async Login(username, password) {
        await this.simulateDelay();

        const users = this.getStoredUsers();
        const user = users.find(u => u.username === username);

        if (!user) {
            return "ERROR: Invalid username or password";
        }

        if (this.simulateVerify(password, user.password)) {
            localStorage.setItem('currentUser', JSON.stringify(user));
            return "SUCCESS: Login successful";
        } else {
            return "ERROR: Invalid username or password";
        }
    }

    async Logout() {
        await this.simulateDelay();
        localStorage.removeItem('currentUser');
    }

    async AddEmployee(name, email, department, hireDate) {
        await this.simulateDelay();

        if (!this.isUserLoggedIn()) {
            return "ERROR: User not logged in";
        }

        const employees = this.getStoredEmployees();
        if (employees.find(e => e.email === email)) {
            return "ERROR: Employee with this email already exists";
        }

        const newEmployee = {
            id: employees.length + 1,
            name,
            email,
            department,
            hireDate: hireDate.toISOString(),
            absentDays: 0
        };

        employees.push(newEmployee);
        this.setStoredEmployees(employees);

        return "SUCCESS: Employee added successfully";
    }

    async GetAllEmployees() {
        await this.simulateDelay();

        if (!this.isUserLoggedIn()) {
            return [];
        }

        const employees = this.getStoredEmployees();
        // Calculate working days for each employee
        return employees.map(emp => ({
            ...emp,
            CalculatedWorkingDays: this.calculateWorkingDays(emp.hireDate, emp.absentDays)
        }));
    }

    async UpdateAbsentDays(employeeId, newAbsentDays) {
        await this.simulateDelay();

        if (!this.isUserLoggedIn()) {
            return "ERROR: User not logged in";
        }

        if (newAbsentDays < 0) {
            return "ERROR: Absent days cannot be negative";
        }

        const employees = this.getStoredEmployees();
        const employee = employees.find(e => e.id === employeeId);

        if (!employee) {
            return "ERROR: Employee not found";
        }

        const totalDaysSinceHire = this.calculateTotalDaysSinceHire(employee.hireDate);
        if (newAbsentDays > totalDaysSinceHire) {
            return `ERROR: Absent days (${newAbsentDays}) cannot exceed total days since hire (${totalDaysSinceHire})`;
        }

        employee.absentDays = newAbsentDays;
        this.setStoredEmployees(employees);

        return "SUCCESS: Absent days updated successfully";
    }

    async ExportEmployeesToCsv() {
        await this.simulateDelay();

        if (!this.isUserLoggedIn()) {
            return "ERROR: User not logged in";
        }

        const employees = await this.GetAllEmployees();
        const headers = "Id,Name,Email,Department,HireDate,AbsentDays,WorkingDays";
        const rows = employees.map(emp =>
            `${emp.id},${emp.name},${emp.email},${emp.department},${new Date(emp.hireDate).toISOString().split('T')[0]},${emp.absentDays},${emp.CalculatedWorkingDays}`
        );

        const csvContent = [headers, ...rows].join('\n');
        return `SUCCESS:${csvContent}`;
    }

    // Helper methods for simulated storage
    getStoredUsers() {
        const stored = localStorage.getItem('users');
        if (stored) {
            return JSON.parse(stored);
        }
        // Default admin user
        const defaultUsers = [{
            id: 1,
            username: 'admin',
            email: 'admin@company.com',
            password: this.simulateHash('admin123')
        }];
        this.setStoredUsers(defaultUsers);
        return defaultUsers;
    }

    setStoredUsers(users) {
        localStorage.setItem('users', JSON.stringify(users));
    }

    getStoredEmployees() {
        const stored = localStorage.getItem('employees');
        return stored ? JSON.parse(stored) : [];
    }

    setStoredEmployees(employees) {
        localStorage.setItem('employees', JSON.stringify(employees));
    }

    isUserLoggedIn() {
        return localStorage.getItem('currentUser') !== null;
    }

    // Simulation of BCrypt-like functionality
    simulateHash(password) {
        // In a real app, this would be proper BCrypt hashing on the server
        return btoa(password + '_hashed_simulated');
    }

    simulateVerify(password, hash) {
        return this.simulateHash(password) === hash;
    }

    calculateWorkingDays(hireDate, absentDays) {
        const totalDays = this.calculateTotalDaysSinceHire(hireDate);
        return Math.max(0, totalDays - absentDays);
    }

    calculateTotalDaysSinceHire(hireDate) {
        const hire = new Date(hireDate);
        const today = new Date();
        const diffTime = today - hire;
        return Math.floor(diffTime / (1000 * 60 * 60 * 24));
    }

    simulateDelay() {
        return new Promise(resolve => setTimeout(resolve, 500));
    }
}