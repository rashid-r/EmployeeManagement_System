class EmployeeManagementApp {
    constructor() {
        this.dbService = new DatabaseService();
        this.currentView = 'login';
        this.employees = [];
        this.init();
    }

    init() {
        this.render();
        this.attachEventListeners();
    }

    render() {
        const appContainer = document.getElementById('app');
        appContainer.innerHTML = this.getCurrentViewTemplate();
    }

    getCurrentViewTemplate() {
        switch (this.currentView) {
            case 'login':
                return this.getLoginTemplate();
            case 'signup':
                return this.getSignupTemplate();
            case 'dashboard':
                return this.getDashboardTemplate();
            default:
                return this.getLoginTemplate();
        }
    }

    getLoginTemplate() {
        return `
            <div class="container">
                <div class="header">
                    <h1>Employee Management System</h1>
                    <p>Please login to continue</p>
                </div>
                <div class="content">
                    <div class="form-container">
                        <div class="form-group">
                            <label for="loginUsername">Username:</label>
                            <input type="text" id="loginUsername" placeholder="Enter your username">
                        </div>
                        <div class="form-group">
                            <label for="loginPassword">Password:</label>
                            <input type="password" id="loginPassword" placeholder="Enter your password">
                        </div>
                        <button class="btn btn-primary" onclick="app.handleLogin()">Login</button>
                        <button class="btn btn-secondary" onclick="app.showSignup()">Sign Up</button>
                    </div>
                </div>
            </div>
        `;
    }

    getSignupTemplate() {
        return `
            <div class="container">
                <div class="header">
                    <h1>Employee Management System</h1>
                    <p>Create a new account</p>
                </div>
                <div class="content">
                    <div class="form-container">
                        <div class="form-group">
                            <label for="signupUsername">Username:</label>
                            <input type="text" id="signupUsername" placeholder="Choose a username">
                        </div>
                        <div class="form-group">
                            <label for="signupEmail">Email:</label>
                            <input type="email" id="signupEmail" placeholder="Enter your email">
                        </div>
                        <div class="form-group">
                            <label for="signupPassword">Password:</label>
                            <input type="password" id="signupPassword" placeholder="Choose a password">
                        </div>
                        <button class="btn btn-primary" onclick="app.handleSignup()">Sign Up</button>
                        <button class="btn btn-secondary" onclick="app.showLogin()">Back to Login</button>
                    </div>
                </div>
            </div>
        `;
    }

    getDashboardTemplate() {
        return `
            <div class="container">
                <div class="header">
                    <h1>Employee Management System</h1>
                    <p>Welcome to your dashboard</p>
                </div>
                <div class="content">
                    <div class="nav-buttons">
                        <button class="btn btn-primary" onclick="app.showAddEmployeeForm()">Add Employee</button>
                        <button class="btn btn-success" onclick="app.handleExport()">Export to CSV</button>
                        <button class="btn btn-danger" onclick="app.handleLogout()">Logout</button>
                    </div>
                    
                    <div id="addEmployeeForm" class="form-container hidden">
                        <h3>Add New Employee</h3>
                        <div class="form-group">
                            <label for="employeeName">Name:</label>
                            <input type="text" id="employeeName" placeholder="Enter employee name">
                        </div>
                        <div class="form-group">
                            <label for="employeeEmail">Email:</label>
                            <input type="email" id="employeeEmail" placeholder="Enter employee email">
                        </div>
                        <div class="form-group">
                            <label for="employeeDepartment">Department:</label>
                            <input type="text" id="employeeDepartment" placeholder="Enter department">
                        </div>
                        <div class="form-group">
                            <label for="employeeHireDate">Hire Date:</label>
                            <input type="date" id="employeeHireDate">
                        </div>
                        <button class="btn btn-primary" onclick="app.handleAddEmployee()">Add Employee</button>
                        <button class="btn btn-secondary" onclick="app.hideAddEmployeeForm()">Cancel</button>
                    </div>

                    <div id="employeesContainer">
                        <h3>Employee List</h3>
                        <div class="employees-grid" id="employeesGrid">
                            ${this.getEmployeesGridTemplate()}
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    getEmployeesGridTemplate() {
        if (this.employees.length === 0) {
            return '<p>No employees found. Add your first employee!</p>';
        }

        return this.employees.map(emp => `
            <div class="employee-card">
                <h3>${this.escapeHtml(emp.Name)}</h3>
                <p><strong>Email:</strong> ${this.escapeHtml(emp.Email)}</p>
                <p><strong>Department:</strong> ${this.escapeHtml(emp.Department)}</p>
                <p><strong>Hire Date:</strong> ${new Date(emp.HireDate).toLocaleDateString()}</p>
                <p><strong>Absent Days:</strong> ${emp.AbsentDays}</p>
                <p><strong>Working Days:</strong> ${emp.CalculatedWorkingDays}</p>
                <div class="employee-actions">
                    <button class="btn btn-primary" onclick="app.showEditAbsentDaysModal(${emp.Id}, ${emp.AbsentDays})">
                        Edit Absent Days
                    </button>
                </div>
            </div>
        `).join('');
    }

    attachEventListeners() {
        // Add enter key support for login/signup forms
        document.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                if (this.currentView === 'login') {
                    this.handleLogin();
                } else if (this.currentView === 'signup') {
                    this.handleSignup();
                }
            }
        });
    }

    async handleLogin() {
        const username = document.getElementById('loginUsername').value;
        const password = document.getElementById('loginPassword').value;

        if (!username || !password) {
            this.showMessage('Please fill in all fields', 'error');
            return;
        }

        const result = await this.dbService.Login(username, password);
        if (result.startsWith('SUCCESS')) {
            this.showMessage('Login successful!', 'success');
            this.currentView = 'dashboard';
            await this.loadEmployees();
            this.render();
        } else {
            this.showMessage(result.replace('ERROR: ', ''), 'error');
        }
    }

    async handleSignup() {
        const username = document.getElementById('signupUsername').value;
        const email = document.getElementById('signupEmail').value;
        const password = document.getElementById('signupPassword').value;

        if (!username || !email || !password) {
            this.showMessage('Please fill in all fields', 'error');
            return;
        }

        const result = await this.dbService.Signup(username, password, email);
        if (result.startsWith('SUCCESS')) {
            this.showMessage('Registration successful! Please login.', 'success');
            this.showLogin();
        } else {
            this.showMessage(result.replace('ERROR: ', ''), 'error');
        }
    }

    async handleAddEmployee() {
        const name = document.getElementById('employeeName').value;
        const email = document.getElementById('employeeEmail').value;
        const department = document.getElementById('employeeDepartment').value;
        const hireDate = document.getElementById('employeeHireDate').value;

        if (!name || !email || !department || !hireDate) {
            this.showMessage('Please fill in all fields', 'error');
            return;
        }

        const result = await this.dbService.AddEmployee(name, email, department, new Date(hireDate));
        if (result.startsWith('SUCCESS')) {
            this.showMessage('Employee added successfully!', 'success');
            this.hideAddEmployeeForm();
            await this.loadEmployees();
            this.renderEmployeesGrid();
        } else {
            this.showMessage(result.replace('ERROR: ', ''), 'error');
        }
    }

    async handleExport() {
        const result = await this.dbService.ExportEmployeesToCsv();
        if (result.startsWith('SUCCESS')) {
            const csvData = result.replace('SUCCESS:', '');
            this.downloadCsv(csvData, 'employees_export.csv');
            this.showMessage('Employees exported successfully!', 'success');
        } else {
            this.showMessage(result.replace('ERROR: ', ''), 'error');
        }
    }

    async handleLogout() {
        await this.dbService.Logout();
        this.showMessage('Logged out successfully', 'info');
        this.currentView = 'login';
        this.employees = [];
        this.render();
    }

    async loadEmployees() {
        this.employees = await this.dbService.GetAllEmployees();
    }

    renderEmployeesGrid() {
        const grid = document.getElementById('employeesGrid');
        if (grid) {
            grid.innerHTML = this.getEmployeesGridTemplate();
        }
    }

    showLogin() {
        this.currentView = 'login';
        this.render();
    }

    showSignup() {
        this.currentView = 'signup';
        this.render();
    }

    showAddEmployeeForm() {
        const form = document.getElementById('addEmployeeForm');
        if (form) {
            form.classList.remove('hidden');
        }
    }

    hideAddEmployeeForm() {
        const form = document.getElementById('addEmployeeForm');
        if (form) {
            form.classList.add('hidden');
            // Clear form fields
            document.getElementById('employeeName').value = '';
            document.getElementById('employeeEmail').value = '';
            document.getElementById('employeeDepartment').value = '';
            document.getElementById('employeeHireDate').value = '';
        }
    }

    showEditAbsentDaysModal(employeeId, currentAbsentDays) {
        const modal = document.createElement('div');
        modal.className = 'modal-overlay';
        modal.innerHTML = `
            <div class="modal-content">
                <div class="modal-header">
                    <h3>Edit Absent Days</h3>
                </div>
                <div class="form-group">
                    <label for="absentDaysInput">Absent Days:</label>
                    <input type="number" id="absentDaysInput" value="${currentAbsentDays}" min="0" step="1">
                </div>
                <div class="modal-footer">
                    <button class="btn btn-primary" onclick="app.handleUpdateAbsentDays(${employeeId})">Update</button>
                    <button class="btn btn-secondary" onclick="app.closeModal()">Cancel</button>
                </div>
            </div>
        `;
        document.body.appendChild(modal);
    }

    async handleUpdateAbsentDays(employeeId) {
        const input = document.getElementById('absentDaysInput');
        const newAbsentDays = parseInt(input.value);

        if (isNaN(newAbsentDays) || newAbsentDays < 0) {
            this.showMessage('Please enter a valid non-negative number', 'error');
            return;
        }

        const result = await this.dbService.UpdateAbsentDays(employeeId, newAbsentDays);
        if (result.startsWith('SUCCESS')) {
            this.showMessage('Absent days updated successfully!', 'success');
            this.closeModal();
            await this.loadEmployees();
            this.renderEmployeesGrid();
        } else {
            this.showMessage(result.replace('ERROR: ', ''), 'error');
        }
    }

    closeModal() {
        const modal = document.querySelector('.modal-overlay');
        if (modal) {
            modal.remove();
        }
    }

    showMessage(message, type = 'info') {
        // Remove existing message boxes
        const existingMessages = document.querySelectorAll('.message-box');
        existingMessages.forEach(msg => msg.remove());

        const messageBox = document.createElement('div');
        messageBox.className = `message-box ${type}`;
        messageBox.textContent = message;

        document.body.appendChild(messageBox);

        // Show with animation
        setTimeout(() => messageBox.classList.add('show'), 100);

        // Auto-hide after 5 seconds
        setTimeout(() => {
            messageBox.classList.remove('show');
            setTimeout(() => messageBox.remove(), 300);
        }, 5000);
    }

    downloadCsv(data, filename) {
        const blob = new Blob([data], { type: 'text/csv' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    }

    escapeHtml(unsafe) {
        return unsafe
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }
}

// Initialize the application
document.addEventListener('DOMContentLoaded', function () {
    window.app = new EmployeeManagementApp();
    window.app.init();
});