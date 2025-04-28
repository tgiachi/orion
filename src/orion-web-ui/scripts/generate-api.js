import { generateApi } from 'swagger-typescript-api';
import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';
import { exec } from 'child_process';
import { promisify } from 'util';

const execAsync = promisify(exec);

// Get current path
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Directory for generated files
const outputDir = path.resolve(__dirname, '../src/api');
const outputFile = path.resolve(outputDir, 'api.ts');
const outputTypes = path.resolve(outputDir, 'types.ts');

// Parse command line arguments
const args = process.argv.slice(2);
let apiUrl = 'http://127.0.0.1:23021/openapi/v1/openapi.json';

// Check if a custom URL is specified
const urlIndex = args.indexOf('--url');
if (urlIndex !== -1 && urlIndex + 1 < args.length) {
  apiUrl = args[urlIndex + 1];
}

console.log(`Generating TypeScript interfaces from: ${apiUrl}`);

// Main function to generate the API
async function generateApiClient() {
  try {
    // Create output directory if it doesn't exist
    if (!fs.existsSync(outputDir)) {
      fs.mkdirSync(outputDir, { recursive: true });
    }

    // Generate API client
    const { files } = await generateApi({
      name: 'OrionApi',
      output: outputDir,
      url: apiUrl,
      generateClient: true,
      generateRouteTypes: true,
      generateResponses: true,
      extractRequestParams: true,
      extractRequestBody: true,
      extractEnums: true,
      toJS: false,
      unwrapResponseData: true,
      moduleNameFirstTag: true,
      singleHttpClient: true,
      cleanOutput: true,
      enumNamesAsValues: true,
      generateUnionEnums: true,
    });

    console.log('API client generated successfully!');

    // Get and save types only in a separate file
    await extractTypesOnly();

    // Format generated files
    try {
      await execAsync(`npx prettier --write "${outputDir}/**/*.ts"`);
      console.log('Files formatted with Prettier');
    } catch (formatError) {
      console.warn('Warning: Unable to format files with Prettier. Check if it is installed.');
      console.warn(formatError.message);
    }

    console.log(`Files generated in: ${outputDir}`);
  } catch (error) {
    console.error('Error generating API:', error);
    process.exit(1);
  }
}

// Function to extract types only using openapi-typescript
async function extractTypesOnly() {
  try {
    // Execute openapi-typescript to generate only type definitions
    const { stdout, stderr } = await execAsync(`npx openapi-typescript ${apiUrl} --output ${outputTypes}`);

    if (stderr) {
      console.warn('Warning during type generation:', stderr);
    }

    console.log('Types extracted successfully!');

    // Add an export statement to make types available to the rest of the application
    let typeContent = fs.readFileSync(outputTypes, 'utf8');
    typeContent = `/**
 * Automatically generated interfaces from OpenAPI
 * Do not modify this file manually
 */

${typeContent}

// Export all types
export * from './types';
`;
    fs.writeFileSync(outputTypes, typeContent);

  } catch (error) {
    console.error('Error extracting types:', error);
  }
}

// Run the script
generateApiClient();
