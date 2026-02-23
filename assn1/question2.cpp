/*
 * Question 2 – DT Dobie (K) Ltd Vehicle Sales System
 * ----------------------------------------------------
 * Class  : Vehicle
 * Database: vehicles.dat (binary flat file)
 *
 * Operations:
 *   (i)   set_vehicle() – capture vehicle details
 *   (ii)  get_profit()  – calculate 15% profit on sale price
 *   (iii) main()        – demonstrate using an object
 *   (iv)  Store records in a file database
 */

#include <iostream>
#include <fstream>
#include <iomanip>
#include <string>
#include <limits>

using namespace std;

// ─────────────────────────────────────────────
//  Vehicle Class
// ─────────────────────────────────────────────
class Vehicle {
private:
    char   make[30];          // e.g. Nissan, Mercedes
    char   model[30];         // e.g. Sunny, C-Class
    char   engine_number[20]; // unique engine identifier
    double sale_price;        // sale price in KES

    static const double PROFIT_RATE; // 15 %

public:
    // ── (i) Capture vehicle details ──────────
    void set_vehicle() {
        string mk, md, en;
        double sp;

        cout << "\n=== Enter Vehicle Details ===\n";
        cout << "  Make          : "; cin.ignore(); getline(cin, mk);
        cout << "  Model         : ";               getline(cin, md);
        cout << "  Engine Number : ";               getline(cin, en);
        cout << "  Sale Price    : "; cin >> sp;

        strncpy(make,          mk.c_str(), 29);  make[29]          = '\0';
        strncpy(model,         md.c_str(), 29);  model[29]         = '\0';
        strncpy(engine_number, en.c_str(), 19);  engine_number[19] = '\0';
        sale_price = sp;
    }

    // ── (ii) Calculate and return 15% profit ─
    double get_profit() const {
        return sale_price * PROFIT_RATE;
    }

    // ── Getters ──────────────────────────────
    string getMake()         const { return string(make);          }
    string getModel()        const { return string(model);         }
    string getEngineNumber() const { return string(engine_number); }
    double getSalePrice()    const { return sale_price;            }

    // ── Save record to binary file ────────────
    void saveToDatabase() const {
        ofstream outFile("vehicles.dat", ios::binary | ios::app);
        if (!outFile) {
            cerr << "  [ERROR] Could not open vehicles.dat for writing.\n";
            return;
        }
        outFile.write(reinterpret_cast<const char*>(this), sizeof(Vehicle));
        outFile.close();
        cout << "  [OK] Vehicle record saved to database.\n";
    }

    // ── Display all records from file ─────────
    static void displayAll() {
        ifstream inFile("vehicles.dat", ios::binary);
        if (!inFile) {
            cout << "\n  [INFO] No records found. Database is empty.\n";
            return;
        }

        Vehicle v;
        int     count = 0;

        cout << "\n=== DT Dobie (K) Ltd – Vehicle Sales Database ===\n";
        cout << left
             << setw(15) << "Make"
             << setw(15) << "Model"
             << setw(20) << "Engine No."
             << setw(18) << "Sale Price (KES)"
             << setw(18) << "Profit (KES)"
             << "\n";
        cout << string(86, '-') << "\n";

        while (inFile.read(reinterpret_cast<char*>(&v), sizeof(Vehicle))) {
            cout << left
                 << setw(15) << v.getMake()
                 << setw(15) << v.getModel()
                 << setw(20) << v.getEngineNumber()
                 << setw(18) << fixed << setprecision(2) << v.getSalePrice()
                 << setw(18) << fixed << setprecision(2) << v.get_profit()
                 << "\n";
            ++count;
        }
        inFile.close();

        if (count == 0)
            cout << "  [INFO] No records found.\n";
        else
            cout << string(86, '-') << "\n"
                 << "  Total vehicles: " << count << "\n";
    }
};

// Define static constant (15%)
const double Vehicle::PROFIT_RATE = 0.15;

// ─────────────────────────────────────────────
//  Main Function
// ─────────────────────────────────────────────
int main() {
    int choice;

    cout << "╔══════════════════════════════════════╗\n";
    cout << "║   DT Dobie (K) Ltd – Vehicle Sales   ║\n";
    cout << "╚══════════════════════════════════════╝\n";

    do {
        cout << "\n--- MENU ---\n"
             << "  1. Add a new vehicle\n"
             << "  2. Display all vehicles\n"
             << "  0. Exit\n"
             << "Enter choice: ";
        cin >> choice;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            choice = -1;
        }

        switch (choice) {
            case 1: {
                Vehicle v;
                v.set_vehicle();

                cout << "\n--- Vehicle Summary ---\n"
                     << "  Make          : " << v.getMake()         << "\n"
                     << "  Model         : " << v.getModel()        << "\n"
                     << "  Engine Number : " << v.getEngineNumber() << "\n"
                     << "  Sale Price    : KES " << fixed << setprecision(2) << v.getSalePrice() << "\n"
                     << "  Profit (15%)  : KES " << fixed << setprecision(2) << v.get_profit()   << "\n";

                v.saveToDatabase();
                break;
            }
            case 2:
                Vehicle::displayAll();
                break;
            case 0:
                cout << "Goodbye!\n";
                break;
            default:
                cout << "  Invalid option. Try again.\n";
        }
    } while (choice != 0);

    return 0;
}
